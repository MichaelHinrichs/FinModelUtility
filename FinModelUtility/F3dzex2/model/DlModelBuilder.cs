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
  public class DlModelBuilder {
    private readonly IN64Hardware n64Hardware_;
    private IMesh currentMesh_;

    public DlModelBuilder(IN64Memory n64Memory) {
      var n64Hardware = new N64Hardware { Memory = n64Memory, };
      this.n64Hardware_ = n64Hardware;

      n64Hardware.Rdp = new Rdp {
          F3dVertices = new F3dVertices(n64Hardware, this.Model),
          Tmem = new JankTmem(n64Hardware, this.Model)
      };
      n64Hardware.Rsp = new Rsp();

      this.currentMesh_ = this.Model.Skin.AddMesh();
    }

    public IModel Model { get; } = new ModelImpl();

    public IReadOnlyFinMatrix4x4 Matrix {
      set => this.n64Hardware_.Rsp.Matrix = value;
    }

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
            this.n64Hardware_.Rsp.GeometryMode |= flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.n64Hardware_.Rdp.Tmem.CullingMode =
                  this.n64Hardware_.Rsp.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case ClearGeometryModeOpcodeCommand clearGeometryModeOpcodeCommand: {
            var flagsToEnable = clearGeometryModeOpcodeCommand.FlagsToDisable;
            this.n64Hardware_.Rsp.GeometryMode &= ~flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.n64Hardware_.Rdp.Tmem.CullingMode =
                  this.n64Hardware_.Rsp.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTile(
                setTileOpcodeCommand.ColorFormat,
                setTileOpcodeCommand.BitsPerTexel,
                0,
                0,
                setTileOpcodeCommand.TileDescriptorIndex,
                setTileOpcodeCommand.WrapModeS,
                setTileOpcodeCommand.WrapModeT);
            break;
          }
          case SetTileSizeOpcodeCommand setTileSizeOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTileSize(0,
              0,
              setTileSizeOpcodeCommand
                  .TileDescriptorIndex,
              setTileSizeOpcodeCommand.Width,
              setTileSizeOpcodeCommand.Height);
            break;
          }
          case SetTimgOpcodeCommand setTimgOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTextureImage(
                setTimgOpcodeCommand.ColorFormat,
                setTimgOpcodeCommand.BitsPerTexel,
                0,
                setTimgOpcodeCommand
                    .TextureSegmentedAddress);
            break;
          }
          case TextureOpcodeCommand textureOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsSpTexture(
                textureOpcodeCommand.HorizontalScaling,
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
            this.n64Hardware_.Rdp.F3dVertices.LoadVertices(
                vtxOpcodeCommand.Vertices,
                vtxOpcodeCommand.IndexToBeginStoringVertices);
            break;
          }
          case Tri1OpcodeCommand tri1OpcodeCommand: {
            var material =
                this.n64Hardware_.Rdp.Tmem.GetOrCreateMaterialForTile0();
            var vertices =
                tri1OpcodeCommand.VertexIndicesInOrder.Select(
                    this.n64Hardware_.Rdp.F3dVertices.GetOrCreateVertexAtIndex);
            this.currentMesh_.AddTriangles(vertices.ToArray())
                .SetMaterial(material)
                .SetVertexOrder(VertexOrder.NORMAL);
            break;
          }
          case LoadBlockOpcodeCommand loadBlockOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpLoadBlock(0,
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

                var jankTmem = this.n64Hardware_.Rdp.JankTmem;
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
  }
}