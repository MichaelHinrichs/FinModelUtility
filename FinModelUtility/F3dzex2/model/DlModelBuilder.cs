using System;
using System.Drawing;
using System.Linq;

using f3dzex2.displaylist;
using f3dzex2.displaylist.opcodes;
using f3dzex2.image;
using f3dzex2.io;

using fin.math;
using fin.math.matrix;
using fin.model;
using fin.model.impl;
using fin.util.enums;


namespace f3dzex2.model {
  public class Rsp {
    public GeometryMode GeometryMode { get; set; } = (GeometryMode) 0x22205;
    public float TexScaleX { get; set; } = 1;
    public float TexScaleY { get; set; } = 1;
  }

  public class DlModelBuilder {
    private readonly IN64Memory n64Memory_;
    private IMesh currentMesh_;

    private readonly Rsp rsp_ = new();
    private readonly ITmem tmem_;

    private const int VERTEX_COUNT = 32;

    private readonly F3dVertex[] vertexDefinitions_ =
        new F3dVertex[VERTEX_COUNT];

    private readonly IVertex?[] vertices_ = new IVertex?[VERTEX_COUNT];

    public DlModelBuilder(IN64Memory n64Memory) {
      this.n64Memory_ = n64Memory;
      this.currentMesh_ = this.Model.Skin.AddMesh();

      this.tmem_ = new JankTmem(this.n64Memory_, this.Model, this.rsp_);
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
            this.rsp_.GeometryMode |= flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.tmem_.CullingMode =
                  this.rsp_.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case ClearGeometryModeOpcodeCommand clearGeometryModeOpcodeCommand: {
            var flagsToEnable = clearGeometryModeOpcodeCommand.FlagsToDisable;
            this.rsp_.GeometryMode &= ~flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.tmem_.CullingMode =
                  this.rsp_.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand: {
            this.tmem_.GsDpSetTile(setTileOpcodeCommand.ColorFormat,
                                   setTileOpcodeCommand.BitsPerTexel,
                                   0,
                                   0,
                                   setTileOpcodeCommand.TileDescriptorIndex,
                                   setTileOpcodeCommand.WrapModeS,
                                   setTileOpcodeCommand.WrapModeT);
            break;
          }
          case SetTileSizeOpcodeCommand setTileSizeOpcodeCommand: {
            this.tmem_.GsDpSetTileSize(0,
                                       0,
                                       setTileSizeOpcodeCommand
                                           .TileDescriptorIndex,
                                       setTileSizeOpcodeCommand.Width,
                                       setTileSizeOpcodeCommand.Height);

            break;
          }
          case SetTimgOpcodeCommand setTimgOpcodeCommand: {
            this.tmem_.GsDpSetTextureImage(setTimgOpcodeCommand.ColorFormat,
                                           setTimgOpcodeCommand.BitsPerTexel,
                                           0,
                                           setTimgOpcodeCommand
                                               .TextureSegmentedAddress);
            break;
          }
          case TextureOpcodeCommand textureOpcodeCommand: {
            this.tmem_.GsSpTexture(textureOpcodeCommand.HorizontalScaling,
                textureOpcodeCommand.VerticalScaling,
                textureOpcodeCommand.MaximumNumberOfMipmaps,
                textureOpcodeCommand.TileDescriptorIndex,
                textureOpcodeCommand.NewTileDescriptorState);

            break;
          }
          case SetCombineOpcodeCommand setCombineOpcodeCommand: {
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
            var material = this.tmem_.GetOrCreateMaterialForTile0();
            var vertices =
                tri1OpcodeCommand.VertexIndicesInOrder.Select(
                    GetOrCreateVertexAtIndex_);
            this.currentMesh_.AddTriangles(vertices.ToArray())
                .SetMaterial(material)
                .SetVertexOrder(VertexOrder.NORMAL);
            break;
          }
          case LoadBlockOpcodeCommand loadBlockOpcodeCommand: {
            this.tmem_.GsDpLoadBlock(0,
                                     0,
                                     loadBlockOpcodeCommand.TileDescriptorIndex,
                                     0,
                                     0);
            break;
          }
          case MoveMemOpcodeCommand moveMemOpcodeCommand: {
            // TODO: How to handle this in a more generalized way?
            switch (moveMemOpcodeCommand.DmemAddress) {
              // Diffuse light
              // https://hack64.net/wiki/doku.php?id=super_mario_64:fast3d_display_list_commands
              case DmemAddress.G_MV_L0: {
                using var er =
                    n64Memory.OpenAtSegmentedAddress(
                        moveMemOpcodeCommand.SegmentedAddress);
                var r = er.ReadByte();
                var g = er.ReadByte();
                var b = er.ReadByte();

                // TODO: Support normalized light direction

                var jankTmem = this.JankTmem;
                if (jankTmem != null) {
                  var loadingTileParams = jankTmem.LoadingTileParams;
                  loadingTileParams.DiffuseColor = Color.FromArgb(r, g, b);
                  jankTmem.LoadingTileParams = loadingTileParams;
                }
                break;
              }
              // Ambient light
              case DmemAddress.G_MV_L1: {
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

      var img =
          this.tmem_.GetOrCreateMaterialForTile0().Textures.First().Image;
      var bmpWidth = Math.Max(img.Width, (ushort) 0);
      var bmpHeight = Math.Max(img.Height, (ushort) 0);

      var newVertex = this.Model.Skin.AddVertex(position)
                          .SetUv(definition.GetUv(
                                     this.rsp_.TexScaleX /
                                     (bmpWidth * 32),
                                     this.rsp_.TexScaleY /
                                     (bmpHeight * 32)));

      if (this.rsp_.GeometryMode.CheckFlag(
              GeometryMode.G_LIGHTING)) {
        var normal = definition.GetNormal();
        GlMatrixUtil.ProjectNormal(Matrix.Impl, ref normal);
        newVertex.SetLocalNormal(normal)
                 // TODO: Get rid of this, seems to come from combiner instead
                 .SetColor(this.JankTmem?.RenderTileParams.DiffuseColor ??
                           Color.White);
      } else {
        newVertex.SetColor(definition.GetColor());
      }

      this.vertices_[index] = newVertex;
      return newVertex;
    }

    public JankTmem? JankTmem => this.tmem_ as JankTmem;
  }
}