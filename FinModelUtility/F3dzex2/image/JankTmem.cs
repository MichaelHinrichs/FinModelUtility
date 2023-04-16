using System;
using System.Drawing;
using System.Linq;

using f3dzex2.displaylist.opcodes;

using fin.data.lazy;
using fin.image;
using fin.model;
using fin.util.hash;
using fin.util.image;

namespace f3dzex2.image {
  public struct JankTileDescriptor : IReadOnlyTileDescriptor {
    public JankTileDescriptor() { }

    public MaterialParams MaterialParams { get; set; } = new();

    public TileDescriptorState State => TileDescriptorState.ENABLED;

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
  ///
  ///   References:
  ///   https://github.com/Emill/n64-fast3d-engine/blob/master/gfx_pc.c#L605
  /// </summary>
  public class JankTmem : ITmem {
    private readonly IN64Hardware n64Hardware_;
    private readonly IModel model_;

    private class TextureToLoad {
      public uint SegmentedAddress { get; set; }
      public BitsPerTexel BitsPerTexel { get; set; }
      public uint TileNumber { get; set; }
    }

    private class LoadedTexture {
      public uint SegmentedAddress { get; set; }
      public uint SizeInBytes { get; set; }
    }

    private class TextureTile {
      public N64ColorFormat ColorFormat { get; set; }
      public BitsPerTexel BitsPerTexel { get; set; }
      public F3dWrapMode WrapModeS { get; set; }
      public F3dWrapMode WrapModeT { get; set; }
      public ushort Uls { get; set; }
      public ushort Ult { get; set; }
      public ushort Lrs { get; set; }
      public ushort Lrt { get; set; }
      public uint LineSizeBytes { get; set; }
    }

    private readonly TextureToLoad textureToLoad_ = new();
    private readonly TextureTile textureTile_ = new();

    private readonly LoadedTexture[] loadedTextures_ = {
        new LoadedTexture(), new LoadedTexture(),
    };

    private readonly bool[] texturesChanged_ = { false, false };

    private IMaterial? activeMaterial_;

    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    private readonly LazyDictionary<TextureParams, ITexture>
        lazyTextureDictionary_;

    private readonly LazyDictionary<MaterialParams, IMaterial>
        lazyMaterialDictionary_;


    public JankTmem(IN64Hardware n64Hardware, IModel model) {
      this.n64Hardware_ = n64Hardware;
      this.model_ = model;

      lazyImageDictionary_ =
          new(imageParams => {
            if (imageParams.IsInvalid) {
              return FinImage.Create1x1FromColor(this.DiffuseColor);
            }

            using var er = this.n64Hardware_.Memory.OpenAtSegmentedAddress(
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

                var color = this.DiffuseColor;
                texture.Name = !imageParams.IsInvalid
                    ? String.Format("0x{0:X8}", textureParams.SegmentedAddress)
                    : $"rgb({color.R}, {color.G}, {color.B})";

                texture.WrapModeU = textureParams.WrapModeS.AsFinWrapMode();
                texture.WrapModeV = textureParams.WrapModeT.AsFinWrapMode();
                texture.UvType = textureParams.UvType;
                texture.UvIndex = textureParams.UvIndex;
                return texture;
              });

      lazyMaterialDictionary_ =
          new(materialParams
                  => {
                var texture0 =
                    this.lazyTextureDictionary_[materialParams.TextureParams0];
                var texture1 =
                    this.lazyTextureDictionary_[materialParams.TextureParams1];
                var finMaterial =
                    this.model_.MaterialManager.AddFixedFunctionMaterial();

                finMaterial.Name = $"[{texture0.Name}]/[{texture1.Name}]";
                finMaterial.CullingMode = materialParams.CullingMode;

                var equations = finMaterial.Equations;

                finMaterial.SetTextureSource(0, texture0);
                finMaterial.SetTextureSource(1, texture1);

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

                if (finMaterial.Textures.Any(
                        texture
                            => ImageUtil.GetTransparencyType(texture.Image) ==
                               ImageTransparencyType.TRANSPARENT)) {
                  finMaterial.SetAlphaCompare(AlphaOp.Or,
                                              AlphaCompareType.Always,
                                              .5f,
                                              AlphaCompareType.Always,
                                              0);
                } else {
                  finMaterial.SetAlphaCompare(AlphaOp.Or,
                                              AlphaCompareType.Greater,
                                              .5f,
                                              AlphaCompareType.Never,
                                              0);
                }

                return finMaterial;
              });
    }

    public void GsDpLoadBlock(ushort uls,
                              ushort ult,
                              TileDescriptorIndex tileDescriptor,
                              ushort texels,
                              ushort deltaTPerScanline) {
      if (tileDescriptor == (TileDescriptorIndex) 1) {
        return;
      }

      // The lrs field rather seems to be number of pixels to load
      var wordSizeShift = this.textureToLoad_.BitsPerTexel.GetWordShift();
      var sizeBytes = (uint) ((texels + 1) << wordSizeShift);
      this.loadedTextures_[this.textureToLoad_.TileNumber].SizeInBytes =
          sizeBytes;
      this.loadedTextures_[this.textureToLoad_.TileNumber].SegmentedAddress =
          this.textureToLoad_.SegmentedAddress;

      this.texturesChanged_[textureToLoad_.TileNumber] = true;
    }

    public void GsDpSetTile(N64ColorFormat colorFormat,
                            BitsPerTexel bitsPerTexel,
                            uint num64BitValuesPerRow,
                            uint offsetOfTextureInTmem,
                            TileDescriptorIndex tileDescriptor,
                            F3dWrapMode wrapModeS,
                            F3dWrapMode wrapModeT) {
      if (tileDescriptor == TileDescriptorIndex.TX_RENDERTILE) {
        this.textureTile_.ColorFormat = colorFormat;
        this.textureTile_.BitsPerTexel = bitsPerTexel;
        this.textureTile_.WrapModeT = wrapModeT;
        this.textureTile_.WrapModeS = wrapModeS;
        this.textureTile_.LineSizeBytes = num64BitValuesPerRow * 8;
        this.texturesChanged_[0] = true;
        this.texturesChanged_[1] = true;
      }

      if (tileDescriptor == TileDescriptorIndex.TX_LOADTILE) {
        this.textureToLoad_.TileNumber = offsetOfTextureInTmem / 256;
      }
    }

    public void GsDpSetTileSize(ushort uls,
                                ushort ult,
                                TileDescriptorIndex tileDescriptor,
                                ushort lrs,
                                ushort lrt) {
      if (tileDescriptor == TileDescriptorIndex.TX_RENDERTILE) {
        this.textureTile_.Uls = uls;
        this.textureTile_.Ult = ult;
        this.textureTile_.Lrs = lrs;
        this.textureTile_.Lrt = lrt;

        this.texturesChanged_[0] = true;
        this.texturesChanged_[1] = true;
      }
    }

    public void GsSpTexture(ushort scaleS,
                            ushort scaleT,
                            uint maxExtraMipmapLevels,
                            TileDescriptorIndex tileDescriptor,
                            TileDescriptorState tileDescriptorState) {
      this.n64Hardware_.Rsp.TexScaleXShort = scaleS;
      this.n64Hardware_.Rsp.TexScaleYShort = scaleT;
    }

    public void GsDpSetTextureImage(N64ColorFormat colorFormat,
                                    BitsPerTexel bitsPerTexel,
                                    ushort width,
                                    uint imageSegmentedAddress) {
      this.textureToLoad_.SegmentedAddress = imageSegmentedAddress;
      this.textureToLoad_.BitsPerTexel = bitsPerTexel;
    }

    public IMaterial GetOrCreateMaterial() {
      if (this.activeMaterial_ != null && this.texturesChanged_.All(t => !t)) {
        return this.activeMaterial_;
      }

      var materialParams = new MaterialParams();
      materialParams.TextureParams0 = GetTextureParamsForTile_(0);
      materialParams.TextureParams1 = GetTextureParamsForTile_(1);
      materialParams.CullingMode = this.cullingMode_;

      this.texturesChanged_[0] = false;
      this.texturesChanged_[1] = false;

      return this.activeMaterial_ =
          this.lazyMaterialDictionary_[materialParams];
    }

    private TextureParams GetTextureParamsForTile_(int index) {
      var loadedTexture = this.loadedTextures_[index];

      var textureParams = new TextureParams();
      textureParams.ColorFormat = this.textureTile_.ColorFormat;
      textureParams.BitsPerTexel = this.textureTile_.BitsPerTexel;

      textureParams.UvType =
          this.n64Hardware_.Rsp.GeometryMode.GetUvType();
      textureParams.UvIndex = index;
      textureParams.SegmentedAddress = loadedTexture.SegmentedAddress;
      textureParams.WrapModeS = this.textureTile_.WrapModeS;
      textureParams.WrapModeT = this.textureTile_.WrapModeT;

      textureParams.Width = (ushort) (textureTile_.LineSizeBytes >>
                                      textureParams.BitsPerTexel
                                                   .GetWordShift());
      textureParams.Height = textureTile_.LineSizeBytes == 0
          ? (ushort) 0
          : (ushort) (loadedTexture.SizeInBytes / textureTile_.LineSizeBytes);

      return textureParams;
    }

    public Color DiffuseColor { get; set; }

    private CullingMode cullingMode_;

    public CullingMode CullingMode {
      set {
        if (this.cullingMode_ != value) {
          this.activeMaterial_ = null;
          this.cullingMode_ = value;
        }
      }
    }
  }
}