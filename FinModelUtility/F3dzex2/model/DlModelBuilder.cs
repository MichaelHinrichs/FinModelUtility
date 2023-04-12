using System;
using System.Linq;

using f3dzex2.displaylist;
using f3dzex2.displaylist.opcodes;
using f3dzex2.io;

using fin.model;
using fin.model.impl;

namespace f3dzex2.model {
  public class DlModelBuilder {
    private IMesh currentMesh_;
    private IMaterial currentMaterial_;

    private GeometryMode geometryMode_;

    private const int VERTEX_COUNT = 32;
    private readonly F3dVertex[] vertexDefinitions_ = new F3dVertex[VERTEX_COUNT];
    private readonly IVertex?[] vertices = new IVertex?[VERTEX_COUNT];

    public DlModelBuilder() {
      this.currentMesh_ = this.Model.Skin.AddMesh();
    }

    public IModel Model { get; } = new ModelImpl();

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
            this.geometryMode_ ^= clearGeometryModeOpcodeCommand.FlagsToDisable;
            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand:
            break;
          case SetTimgOpcodeCommand setTimgOpcodeCommand:
            break;
          case TextureOpcodeCommand textureOpcodeCommand:
            break;
          case VtxOpcodeCommand vtxOpcodeCommand: {
            var newVertices = vtxOpcodeCommand.Vertices;
            for (var i = 0; i < newVertices.Count; ++i) {
              this.vertexDefinitions_[
                      vtxOpcodeCommand.IndexToBeginStoringVertices + i] =
                  newVertices[i];
              this.vertices[i] = null;
            }

            break;
          }
          case Tri1OpcodeCommand tri1OpcodeCommand: {
            var vertices =
                tri1OpcodeCommand.VertexIndicesInOrder.Select(
                    GetOrCreateVertexAtIndex_);
            var triangle = this.currentMesh_.AddTriangles(vertices.ToArray());
            triangle.SetMaterial(this.currentMaterial_);
            break;
          }
          default:
            throw new ArgumentOutOfRangeException(nameof(opcodeCommand));
        }
      }
    }

    private IVertex GetOrCreateVertexAtIndex_(byte index) {
      var existing = this.vertices[index];
      if (existing != null) {
        return existing;
      }

      var definition = this.vertexDefinitions_[index];
      var newVertex = this.Model.Skin.AddVertex(definition.GetPosition());

      // TODO: Add UV, color, normal

      this.vertices[index] = newVertex;
      return newVertex;
    }
  }
}