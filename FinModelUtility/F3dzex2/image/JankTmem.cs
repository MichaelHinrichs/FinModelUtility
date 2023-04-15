using System;

using f3dzex2.displaylist.opcodes;
using f3dzex2.io;
using f3dzex2.model;

using fin.data.lazy;
using fin.image;
using fin.model;
using fin.util.hash;

namespace f3dzex2.image {
  public struct JankTileDescriptor : IReadOnlyTileDescriptor {
    public JankTileDescriptor() { }

    public MaterialParams MaterialParams { get; set; } = new();

    public TileDescriptorState State => TileDescriptorState.ENABLED;

    public N64ColorFormat ColorFormat => this.MaterialParams.ColorFormat;
    public BitsPerTexel BitsPerTexel => this.MaterialParams.BitsPerTexel;

    public override int GetHashCode()
      => FluentHash.Start().With(this.State).With(MaterialParams);

    public override bool Equals(object? obj) {
      if (ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is JankTileDescriptor otherTileDescriptor) {
        return this.State == otherTileDescriptor.State &&
               this.MaterialParams.Equals(otherTileDescriptor.MaterialParams);
      }

      return false;
    }
  }

  /// <summary>
  ///   This is a janky implementation of TMEM that picks and chooses which
  ///   params to use based on the tile descriptor. This approach sort of kind
  ///   of works for general cases, but isn't very accurate in general.
  /// </summary>
  public class JankTmem : ITmem {
    private readonly IModel model_;
    private readonly Rsp rsp_;

    private IMaterial? activeMaterial_;
    private bool hasRenderTileParams_ = false;

    private MaterialParams renderTileParams_;
    private MaterialParams loadingTileParams_;

    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    private readonly LazyDictionary<TextureParams, ITexture>
        lazyTextureDictionary_;

    private readonly LazyDictionary<MaterialParams, IMaterial>
        lazyMaterialDictionary_;


    public JankTmem(IN64Memory n64Memory, IModel model, Rsp rsp) {
      this.model_ = model;
      this.rsp_ = rsp;

      lazyImageDictionary_ =
          new(imageParams => {
            if (imageParams.IsInvalid) {
              return FinImage.Create1x1FromColor(imageParams.Color);
            }

            using var er = n64Memory.OpenAtSegmentedAddress(
                imageParams.SegmentedAddress);
            var imageData =
                er.ReadBytes(imageParams.Width *
                             imageParams.Height * 4);

            return new N64ImageParser().Parse(imageParams.ColorFormat,
                                              imageParams.BitsPerTexel,
                                              imageData,
                                              imageParams.Width,
                                              imageParams.Height,
                                              new ushort[] { },
                                              false);
          });

      lazyTextureDictionary_ =
          new(textureParams
                  => {
                var imageParams = textureParams.ImageParams;
                var texture = this.model_.MaterialManager.CreateTexture(
                    this.lazyImageDictionary_[imageParams]);

                var color = imageParams.Color;
                texture.Name = !imageParams.IsInvalid
                    ? String.Format("0x{0:X8}", textureParams.SegmentedAddress)
                    : $"rgb({color.R}, {color.G}, {color.B})";

                texture.WrapModeU = textureParams.WrapModeS.AsFinWrapMode();
                texture.WrapModeV = textureParams.WrapModeT.AsFinWrapMode();
                texture.UvType = textureParams.UvType;
                return texture;
              });

      lazyMaterialDictionary_ =
          new(materialParams
                  => {
                var texture =
                    this.lazyTextureDictionary_[materialParams.TextureParams];
                var finMaterial =
                    this.model_.MaterialManager.AddFixedFunctionMaterial();

                finMaterial.Name = texture.Name;
                finMaterial.CullingMode = materialParams.CullingMode;

                var equations = finMaterial.Equations;

                finMaterial.SetTextureSource(0, texture);

                var color0 = equations.CreateColorConstant(0);
                var scalar1 = equations.CreateScalarConstant(1);

                var vertexColor0 = equations.CreateColorInput(
                    FixedFunctionSource.VERTEX_COLOR_0,
                    color0);
                var textureColor0 = equations.CreateColorInput(
                    FixedFunctionSource.TEXTURE_COLOR_0,
                    color0);

                var vertexAlpha0 =
                    equations.CreateScalarInput(
                        FixedFunctionSource.VERTEX_ALPHA_0,
                        scalar1);
                var textureAlpha0 = equations.CreateScalarInput(
                    FixedFunctionSource.TEXTURE_ALPHA_0,
                    scalar1);

                equations.CreateColorOutput(
                    FixedFunctionSource.OUTPUT_COLOR,
                    vertexColor0.Multiply(textureColor0));
                equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                             vertexAlpha0.Multiply(
                                                 textureAlpha0));

                finMaterial.SetAlphaCompare(AlphaOp.Or,
                                            AlphaCompareType.Greater,
                                            .5f,
                                            AlphaCompareType.Never,
                                            0);

                return finMaterial;
              });
    }

    public void GsDpLoadBlock(ushort uls,
                              ushort ult,
                              TileDescriptorIndex tileDescriptor,
                              ushort texels,
                              ushort deltaTPerScanline) {
      this.MarkMaterialDirty_();
    }

    public void GsDpSetTile(N64ColorFormat colorFormat,
                            BitsPerTexel bitsPerTexel,
                            uint num64BitValuesPerRow,
                            uint offsetOfTextureInTmem,
                            TileDescriptorIndex tileDescriptor,
                            F3dWrapMode wrapModeS,
                            F3dWrapMode wrapModeT) {
      if (tileDescriptor.IsRenderAndAssertNotLoad()) {
        this.loadingTileParams_.ColorFormat = colorFormat;
        this.loadingTileParams_.BitsPerTexel = bitsPerTexel;
      }

      this.loadingTileParams_.WrapModeT = wrapModeT;
      this.loadingTileParams_.WrapModeS = wrapModeS;
    }

    public void GsDpSetTileSize(ushort uls,
                                ushort ult,
                                TileDescriptorIndex tileDescriptor,
                                ushort width,
                                ushort height) {
      if (tileDescriptor.IsRenderAndAssertNotLoad()) {
        this.loadingTileParams_.Width = width;
        this.loadingTileParams_.Height = height;
      }
    }

    public void GsSpTexture(ushort scaleS,
                            ushort scaleT,
                            uint maxExtraMipmapLevels,
                            TileDescriptorIndex tileDescriptor,
                            TileDescriptorState tileDescriptorState) {
      if (tileDescriptor.IsRenderAndAssertNotLoad()) {
        var tsX = scaleS;
        var tsY = scaleT;

        if (tsX != 0xFFFF)
          this.rsp_.TexScaleX = tsX / 65536.0f;
        else
          this.rsp_.TexScaleX = 1.0f;
        if (tsY != 0xFFFF)
          this.rsp_.TexScaleY = tsY / 65536.0f;
        else
          this.rsp_.TexScaleY = 1.0f;
      }
    }

    public void GsDpSetTextureImage(N64ColorFormat colorFormat,
                                    BitsPerTexel bitsPerTexel,
                                    ushort width,
                                    uint imageSegmentedAddress) {
      this.loadingTileParams_.SegmentedAddress = imageSegmentedAddress;
    }

    public IReadOnlyTileDescriptor Tile0
      => new JankTileDescriptor { MaterialParams = this.renderTileParams_ };


    public void MarkMaterialDirty_() {
      this.activeMaterial_ = null;
      this.hasRenderTileParams_ = false;
    }

    public IMaterial GetOrCreateMaterialForTile0() {
      if (this.activeMaterial_ != null) {
        return this.activeMaterial_;
      }

      if (!this.hasRenderTileParams_) {
        this.renderTileParams_ = this.loadingTileParams_;
      }

      this.renderTileParams_.CullingMode =
          this.rsp_.GeometryMode.GetCullingModeNonEx2();
      this.renderTileParams_.UvType = this.rsp_.GeometryMode.GetUvType();

      this.hasRenderTileParams_ = true;
      return this.activeMaterial_ =
          this.lazyMaterialDictionary_[this.renderTileParams_];
    }

    public MaterialParams RenderTileParams => this.renderTileParams_;

    public MaterialParams LoadingTileParams {
      get => this.loadingTileParams_;
      set => this.loadingTileParams_ = value;
    }

    public CullingMode CullingMode {
      set {
        this.loadingTileParams_.CullingMode = value;
        if (this.renderTileParams_.CullingMode != value) {
          this.activeMaterial_ = null;
          this.renderTileParams_.CullingMode = value;
        }
      }
    }
  }
}