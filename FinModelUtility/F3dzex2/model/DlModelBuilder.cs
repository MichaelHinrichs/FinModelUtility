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

using IImage = fin.image.IImage;
using TileDescriptor = f3dzex2.displaylist.opcodes.TileDescriptor;


namespace f3dzex2.model {
  public class DlModelBuilder {
    private readonly IN64Memory n64Memory_;
    private IMesh currentMesh_;

    private IMaterial? activeMaterial_;
    private GeometryMode geometryMode_ = (GeometryMode) 0x22205;
    private TextureParams textureParamsForActiveMaterial_ = new();

    private bool hasActiveMaterialParams_ = false;
    private MaterialParams activeMaterialParams_ = new();
    private MaterialParams wipMaterialParams_ = new();

    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    private readonly LazyDictionary<TextureParams, ITexture>
        lazyTextureDictionary_;

    private readonly LazyDictionary<MaterialParams, IMaterial>
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
              return FinImage.Create1x1FromColor(imageParams.Color);
            }

            using var er =
                this.n64Memory_.OpenAtSegmentedAddress(
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
                var texture = this.Model.MaterialManager.CreateTexture(
                    this.lazyImageDictionary_[textureParams.ImageParams]);
                texture.Name =
                    String.Format("0x{0:X8}", textureParams.SegmentedAddress);
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
                    Model.MaterialManager.AddFixedFunctionMaterial();

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
            var flagsToEnable = setGeometryModeOpcodeCommand.FlagsToEnable;
            this.geometryMode_ |= flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.MarkMaterialDirty_(false);
            }

            break;
          }
          case ClearGeometryModeOpcodeCommand clearGeometryModeOpcodeCommand: {
            var flagsToEnable = clearGeometryModeOpcodeCommand.FlagsToDisable;
            this.geometryMode_ &= ~flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.MarkMaterialDirty_(false);
            }
            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand: {
            if (setTileOpcodeCommand.TileDescriptor ==
                TileDescriptor.TX_RENDERTILE) {
              this.wipMaterialParams_.ColorFormat =
                  setTileOpcodeCommand.ColorFormat;
              this.wipMaterialParams_.BitsPerTexel =
                  setTileOpcodeCommand.BitsPerTexel;

              this.wipMaterialParams_.WrapModeT = setTileOpcodeCommand.WrapModeT;
              this.wipMaterialParams_.WrapModeS = setTileOpcodeCommand.WrapModeS;
            }

            break;
          }
          case SetTileSizeOpcodeCommand setTileSizeOpcodeCommand: {
            if (setTileSizeOpcodeCommand.TileDescriptor ==
                TileDescriptor.TX_RENDERTILE) {
              this.wipMaterialParams_.Width = setTileSizeOpcodeCommand.Width;
              this.wipMaterialParams_.Height = setTileSizeOpcodeCommand.Height;
            }

            break;
          }
          case SetTimgOpcodeCommand setTimgOpcodeCommand: {
            this.wipMaterialParams_.SegmentedAddress =
                setTimgOpcodeCommand.TextureSegmentedAddress;
            break;
          }
          case TextureOpcodeCommand textureOpcodeCommand: {
            if (textureOpcodeCommand.TileDescriptor ==
                TileDescriptor.TX_RENDERTILE) {
              var tsX = textureOpcodeCommand.HorizontalScaling;
              var tsY = textureOpcodeCommand.VerticalScaling;

              if (tsX != 0xFFFF)
                this.wipMaterialParams_.TexScaleX = (float) tsX / 65536.0f;
              else
                this.wipMaterialParams_.TexScaleX = 1.0f;
              if (tsY != 0xFFFF)
                this.wipMaterialParams_.TexScaleY = (float) tsY / 65536.0f;
              else
                this.wipMaterialParams_.TexScaleY = 1.0f;
            }

            break;
          }
          case SetCombineOpcodeCommand setCombineOpcodeCommand: {
            if (setCombineOpcodeCommand.ClearTextureSegmentedAddress) {
              this.wipMaterialParams_.SegmentedAddress = 0;
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
          case LoadBlockOpcodeCommand: {
            this.MarkMaterialDirty_(true);
            break;
          }
          case MoveMemOpcodeCommand moveMemOpcodeCommand: {
            switch (moveMemOpcodeCommand.MoveMemType) {
              case MoveMemType.COLOR: {
                using var er =
                    n64Memory.OpenAtSegmentedAddress(
                        moveMemOpcodeCommand.SegmentedAddress);
                var r = er.ReadByte();
                var g = er.ReadByte();
                var b = er.ReadByte();
                this.wipMaterialParams_.Color = Color.FromArgb(r, g, b);

                break;
              }
            }

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

      var bmpWidth =
          Math.Max(this.activeMaterialParams_.Width, (ushort) 0);
      var bmpHeight =
          Math.Max(this.activeMaterialParams_.Height, (ushort) 0);

      var newVertex = this.Model.Skin.AddVertex(position)
                          .SetUv(definition.GetUv(
                                     this.activeMaterialParams_.TexScaleX /
                                     (bmpWidth * 32),
                                     this.activeMaterialParams_.TexScaleY /
                                     (bmpHeight * 32)));

      if (this.geometryMode_.CheckFlag(
              GeometryMode.G_LIGHTING)) {
        var normal = definition.GetNormal();
        GlMatrixUtil.ProjectNormal(Matrix.Impl, ref normal);
        newVertex.SetLocalNormal(normal)
                 // TODO: Get rid of this, seems to come from combiner instead
                 .SetColor(this.activeMaterialParams_.Color);
      } else {
        newVertex.SetColor(definition.GetColor());
      }

      this.vertices_[index] = newVertex;
      return newVertex;
    }

    public void MarkMaterialDirty_(bool includeParams) {
      this.activeMaterial_ = null;
      if (includeParams) {
        this.hasActiveMaterialParams_ = false;
      }
      this.ClearVertices_();
    }

    public IMaterial GetOrCreateMaterial_() {
      if (this.activeMaterial_ != null) {
        return this.activeMaterial_;
      }

      if (!this.hasActiveMaterialParams_) {
        this.activeMaterialParams_ = this.wipMaterialParams_;
      }
      this.activeMaterialParams_.CullingMode =
          this.geometryMode_.GetCullingModeNonEx2();

      this.hasActiveMaterialParams_ = true;
      return this.activeMaterial_ =
          this.lazyMaterialDictionary_[this.activeMaterialParams_];
    }
  }
}