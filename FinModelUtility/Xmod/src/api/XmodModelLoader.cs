using fin.color;
using fin.io;
using fin.model;
using fin.model.impl;

using xmod.schema;

using PrimitiveType = xmod.schema.PrimitiveType;


namespace xmod.api {
  public class XmodModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.XmodFile;
    public required IFileHierarchyFile XmodFile { get; init; }
  }

  public class XmodModelLoader : IModelLoader<XmodModelFileBundle> {
    public IModel LoadModel(XmodModelFileBundle modelFileBundle) {
      using var tr =
          new FinTextReader(modelFileBundle.XmodFile.Impl.OpenRead());

      var xmod = new Xmod();
      xmod.Read(tr);

      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      foreach (var packet in xmod.Packets) {
        var packetVertices = packet.Adjuncts.Select(adjunct => {
                                     var position =
                                         xmod.Positions[adjunct.PositionIndex];
                                     var normal =
                                         xmod.Normals[adjunct.NormalIndex];
                                     var color =
                                         xmod.Colors[adjunct.ColorIndex];
                                     var uv1 = xmod.Uv1s[adjunct.Uv1Index];

                                     return finSkin
                                            .AddVertex(position.X,
                                              position.Y,
                                              position.Z)
                                            .SetLocalNormal(normal.X,
                                              normal.Y,
                                              normal.Z)
                                            .SetColor(
                                                FinColor.FromRgbaFloats(
                                                    color.X,
                                                    color.Y,
                                                    color.Z,
                                                    color.W))
                                            .SetUv(uv1.X, uv1.Y);
                                   })
                                   .ToArray();

        foreach (var primitive in packet.Primitives) {
          var primitiveVertices =
              primitive.VertexIndices
                       .Skip(primitive.Type switch {
                           PrimitiveType.TRIANGLES => 0,
                           _                       => 1,
                       })
                       .Select(vertexIndex => packetVertices[vertexIndex])
                       .ToArray();
          var finPrimitive = primitive.Type switch {
              PrimitiveType.TRIANGLE_STRIP_1 => finMesh.AddTriangleStrip(
                  primitiveVertices),
              PrimitiveType.TRIANGLE_STRIP_2 => finMesh.AddTriangleStrip(
                  primitiveVertices),
              PrimitiveType.TRIANGLES =>
                  finMesh.AddTriangles(primitiveVertices),
          };

          finPrimitive?.SetVertexOrder(VertexOrder.NORMAL);
        }
      }

      return finModel;
    }
  }
}