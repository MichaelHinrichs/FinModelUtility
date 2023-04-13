using System;
using System.Linq;

using f3dzex2.displaylist;
using f3dzex2.displaylist.opcodes;
using f3dzex2.io;

using fin.math;
using fin.math.matrix;
using fin.model;
using fin.model.impl;
using fin.util.enums;

namespace f3dzex2.model {
  public class DlModelBuilder {
    private IMesh currentMesh_;
    private IMaterial currentMaterial_;

    private GeometryMode geometryMode_;

    private const int VERTEX_COUNT = 32;

    private readonly F3dVertex[] vertexDefinitions_ =
        new F3dVertex[VERTEX_COUNT];

    private readonly IVertex?[] vertices = new IVertex?[VERTEX_COUNT];

    public DlModelBuilder() {
      this.currentMesh_ = this.Model.Skin.AddMesh();
      this.currentMaterial_ = this.Model.MaterialManager.AddNullMaterial();
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
            this.currentMesh_.AddTriangles(vertices.ToArray())
                .SetMaterial(this.currentMaterial_)
                .SetVertexOrder(VertexOrder.NORMAL);
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

      var position = definition.GetPosition();
      GlMatrixUtil.ProjectPosition(Matrix.Impl, ref position);

      var newVertex = this.Model.Skin.AddVertex(position);

      // TODO: Add UV, color

      if (this.geometryMode_.CheckFlag(GeometryMode.G_LIGHTING)) {
        var normal = definition.GetNormal();
        GlMatrixUtil.ProjectNormal(Matrix.Impl, ref normal);
        newVertex.SetLocalNormal(normal);
      }

      this.vertices[index] = newVertex;
      return newVertex;
    }
  }
}