using System;
using System.Drawing;
using System.Linq;

using f3dzex2.displaylist;
using f3dzex2.displaylist.opcodes;
using f3dzex2.image;
using f3dzex2.io;

using fin.data.lazy;
using fin.image;
using fin.math;
using fin.math.matrix;
using fin.model;
using fin.model.impl;
using fin.util.enums;
using fin.util.hash;

using IImage = fin.image.IImage;


namespace f3dzex2.model {
  public struct TextureParams {
    public TextureParams() { }

    public ImageParams ImageParams { get; private set; } = new();

    public N64ColorFormat ColorFormat {
      get => this.ImageParams.ColorFormat;
      set {
        ImageParams imageParams = this.ImageParams;
        imageParams.ColorFormat = value;
        this.ImageParams = imageParams;
      }
    }

    public BitsPerPixel BitsPerPixel {
      get => this.ImageParams.BitsPerPixel;
      set {
        ImageParams imageParams = this.ImageParams;
        imageParams.BitsPerPixel = value;
        this.ImageParams = imageParams;
      }
    }

    public ushort Width {
      get => this.ImageParams.Width;
      set {
        ImageParams imageParams = this.ImageParams;
        imageParams.Width = value;
        this.ImageParams = imageParams;
      }
    }

    public ushort Height {
      get => this.ImageParams.Height;
      set {
        ImageParams imageParams = this.ImageParams;
        imageParams.Height = value;
        this.ImageParams = imageParams;
      }
    }

    public uint SegmentedAddress {
      get => this.ImageParams.SegmentedAddress;
      set {
        ImageParams imageParams = this.ImageParams;
        imageParams.SegmentedAddress = value;
        this.ImageParams = imageParams;
      }
    }

    public F3dWrapMode WrapModeT { get; set; } = F3dWrapMode.REPEAT;
    public F3dWrapMode WrapModeS { get; set; } = F3dWrapMode.REPEAT;

    public override int GetHashCode() => FluentHash.Start()
                                                   .With(this.ImageParams)
                                                   .With(this.WrapModeT)
                                                   .With(this.WrapModeS);

    public override bool Equals(object? other) {
      if (ReferenceEquals(this, other)) {
        return true;
      }

      if (other is TextureParams otherTextureParams) {
        return ImageParams.Equals(otherTextureParams.ImageParams) &&
               this.WrapModeT == otherTextureParams.WrapModeT &&
               this.WrapModeS == otherTextureParams.WrapModeS;
      }

      return false;
    }
  }

  public class DlModelBuilder {
    private readonly IN64Memory n64Memory_;
    private IMesh currentMesh_;
    private IMaterial? currentMaterial_;

    private GeometryMode geometryMode_ = (GeometryMode) 0x22205;
    private TextureParams wipTextureParams_ = new();
    private TextureParams textureParams_ = new();
    private float texScaleX_ = 1f, texScaleY_ = 1f;

    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    private readonly LazyDictionary<TextureParams, ITexture>
        lazyTextureDictionary_;

    private readonly LazyDictionary<TextureParams, IMaterial>
        lazyMaterialDictionary_;

    private const int VERTEX_COUNT = 32;

    private readonly F3dVertex[] vertexDefinitions_ =
        new F3dVertex[VERTEX_COUNT];

    private readonly IVertex?[] vertices_ = new IVertex?[VERTEX_COUNT];

    public DlModelBuilder(IN64Memory n64Memory) {
      this.n64Memory_ = n64Memory;
      this.currentMesh_ = this.Model.Skin.AddMesh();

      lazyImageDictionary_ =
          new(imageParams => {
            if (imageParams.IsInvalid) {
              return FinImage.Create1x1FromColor(Color.White);
            }

            using var er =
                this.n64Memory_.OpenAtSegmentedAddress(
                    imageParams.SegmentedAddress);
            var imageData =
                er.ReadBytes(imageParams.Width *
                             imageParams.Height * 4);

            return new N64ImageParser().Parse(imageParams.ColorFormat,
                                              imageParams.BitsPerPixel,
                                              imageData,
                                              imageParams.Width,
                                              imageParams.Height,
                                              new ushort[] { },
                                              false);
          });

      lazyTextureDictionary_ =
          new(textureParams
                  => {
                var texture = this.Model.MaterialManager.CreateTexture(
                    this.lazyImageDictionary_[textureParams.ImageParams]);
                texture.Name =
                    String.Format("0x{0:X8}", textureParams.SegmentedAddress);
                texture.WrapModeU = textureParams.WrapModeT.AsFinWrapMode();
                texture.WrapModeV = textureParams.WrapModeS.AsFinWrapMode();
                return texture;
              });

      lazyMaterialDictionary_ =
          new(textureParams
                  => {
                var texture = this.lazyTextureDictionary_[textureParams];
                var finMaterial =
                    Model.MaterialManager.AddFixedFunctionMaterial();
                finMaterial.Name = texture.Name;

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

    public IModel Model { get; } = new ModelImpl();

    public IReadOnlyFinMatrix4x4 Matrix { get; set; } = FinMatrix4x4.IDENTITY;

    public int GetNumberOfTriangles() =>
        this.Model.Skin.Meshes
            .SelectMany(mesh => mesh.Primitives)
            .Select(primitive => primitive.Vertices.Count / 3)
            .Sum();

    public void AddDl(IDisplayList dl, IN64Memory n64Memory) {
      foreach (var opcodeCommand in dl.OpcodeCommands) {
        switch (opcodeCommand) {
          case NoopOpcodeCommand _:
            break;
          case DlOpcodeCommand dlOpcodeCommand: {
            foreach (var childDl in dlOpcodeCommand.PossibleBranches) {
              AddDl(childDl, n64Memory);
            }

            if (!dlOpcodeCommand.PushCurrentDlToStack) {
              return;
            }

            break;
          }
          case EndDlOpcodeCommand _: {
            return;
          }
          case MtxOpcodeCommand mtxOpcodeCommand:
            break;
          case PopMtxOpcodeCommand popMtxOpcodeCommand:
            break;
          case SetEnvColorOpcodeCommand setEnvColorOpcodeCommand:
            break;
          case SetFogColorOpcodeCommand setFogColorOpcodeCommand:
            break;
          // Geometry mode commands
          case SetGeometryModeOpcodeCommand setGeometryModeOpcodeCommand: {
            this.geometryMode_ |= setGeometryModeOpcodeCommand.FlagsToEnable;
            break;
          }
          case ClearGeometryModeOpcodeCommand clearGeometryModeOpcodeCommand: {
            this.geometryMode_ &=
                ~clearGeometryModeOpcodeCommand.FlagsToDisable;
            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand: {
            // TODO: Match returning/control flow logic from Fast3DScripts version
            if (setTileOpcodeCommand.TileDescriptor ==
                TileDescriptor.TX_RENDERTILE) {
              this.textureParams_.ColorFormat =
                  setTileOpcodeCommand.ColorFormat;
              this.textureParams_.BitsPerPixel =
                  setTileOpcodeCommand.BitsPerPixel;

              this.textureParams_.WrapModeT = setTileOpcodeCommand.WrapModeT;
              this.textureParams_.WrapModeS = setTileOpcodeCommand.WrapModeS;
              this.currentMaterial_ = null;
            }

            break;
          }
          case SetTileSizeOpcodeCommand setTileSizeOpcodeCommand: {
            this.textureParams_.Width = setTileSizeOpcodeCommand.Width;
            this.textureParams_.Height = setTileSizeOpcodeCommand.Height;
            this.ClearVertices_();
            break;
          }
          case SetTimgOpcodeCommand setTimgOpcodeCommand: {
            this.textureParams_.SegmentedAddress =
                setTimgOpcodeCommand.TextureSegmentedAddress;
            break;
          }
          case TextureOpcodeCommand textureOpcodeCommand: {
            var tsX = textureOpcodeCommand.HorizontalScaling;
            var tsY = textureOpcodeCommand.VerticalScaling;

            if (this.geometryMode_.HasFlag(GeometryMode.G_TEXTURE_GEN)) {
              this.textureParams_.Width = (ushort) ((tsX >> 6));
              this.textureParams_.Height = (ushort) ((tsY >> 6));
              if (this.textureParams_.Width == 31)
                this.textureParams_.Width = 32;
              else if (this.textureParams_.Width == 62)
                this.textureParams_.Width = 64;
              if (this.textureParams_.Height == 31)
                this.textureParams_.Height = 32;
              else if (this.textureParams_.Height == 62)
                this.textureParams_.Height = 64;
            } else {
              if (tsX != 0xFFFF)
                texScaleX_ = (float) tsX / 65536.0f;
              else
                texScaleX_ = 1.0f;
              if (tsY != 0xFFFF)
                texScaleY_ = (float) tsY / 65536.0f;
              else
                texScaleY_ = 1.0f;
            }

            this.ClearVertices_();
            break;
          }
          case SetCombineOpcodeCommand setCombineOpcodeCommand: {
            if (setCombineOpcodeCommand.ClearTextureSegmentedAddress) {
              this.textureParams_.SegmentedAddress = 0;
            }

            break;
          }
          case VtxOpcodeCommand vtxOpcodeCommand: {
            var newVertices = vtxOpcodeCommand.Vertices;
            for (var i = 0; i < newVertices.Count; ++i) {
              var index = vtxOpcodeCommand.IndexToBeginStoringVertices + i;
              this.vertexDefinitions_[index] =
                  newVertices[i];
              this.vertices_[index] = null;
            }

            break;
          }
          case Tri1OpcodeCommand tri1OpcodeCommand: {
            var material = this.GetOrCreateMaterial_();
            var vertices =
                tri1OpcodeCommand.VertexIndicesInOrder.Select(
                    GetOrCreateVertexAtIndex_);
            this.currentMesh_.AddTriangles(vertices.ToArray())
                .SetMaterial(material)
                .SetVertexOrder(VertexOrder.NORMAL);
            break;
          }
          default:
            throw new ArgumentOutOfRangeException(nameof(opcodeCommand));
        }
      }
    }

    private void ClearVertices_() {
      for (var i = 0; i < vertices_.Length; ++i) {
        this.vertices_[i] = null;
      }
    }

    private IVertex GetOrCreateVertexAtIndex_(byte index) {
      var existing = this.vertices_[index];
      if (existing != null) {
        return existing;
      }

      var definition = this.vertexDefinitions_[index];

      var position = definition.GetPosition();
      GlMatrixUtil.ProjectPosition(Matrix.Impl, ref position);

      var bmp = this.currentMaterial_?.Textures.First().Image;
      var bmpWidth = bmp?.Width ?? 1;
      var bmpHeight = bmp?.Height ?? 1;

      var newVertex = this.Model.Skin.AddVertex(position)
                          .SetUv(definition.GetUv(
                                     this.texScaleX_ / (bmpWidth * 32),
                                     this.texScaleY_ / (bmpHeight * 32)));

      if (this.geometryMode_.CheckFlag(GeometryMode.G_LIGHTING)) {
        var normal = definition.GetNormal();
        GlMatrixUtil.ProjectNormal(Matrix.Impl, ref normal);
        newVertex.SetLocalNormal(normal);

        // TODO: Support color in this case
      } else {
        newVertex.SetColor(definition.GetColor());
      }

      this.vertices_[index] = newVertex;
      return newVertex;
    }

    public IMaterial GetOrCreateMaterial_() {
      if (this.currentMaterial_ != null) {
        return this.currentMaterial_;
      }

      return this.currentMaterial_ =
          this.lazyMaterialDictionary_[this.textureParams_];
    }
  }
}