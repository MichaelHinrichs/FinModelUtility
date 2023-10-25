using Assimp.Unmanaged;

using dat.schema;

using fin.io;
using fin.model;
using fin.model.impl;
using fin.model.io.importers;

using gx;

using schema.binary;

namespace dat.api {
  public class DatModelImporter : IModelImporter<DatModelFileBundle> {
    public IModel ImportModel(DatModelFileBundle modelFileBundle) {
      var dat =
          modelFileBundle.PrimaryDatFile.ReadNew<Dat>(Endianness.BigEndian);

      var finModel = new ModelImpl();

      // Adds skeleton
      var boneQueue = new Queue<(IBone finParentBone, JObj datBone)>();
      foreach (var datRootBone in dat.RootJObjs) {
        boneQueue.Enqueue((finModel.Skeleton.Root, datRootBone));
      }

      while (boneQueue.Count > 0) {
        var (finParentBone, datBone) = boneQueue.Dequeue();

        var datBoneData = datBone.Data;

        var finBone =
            finParentBone.AddChild(datBoneData.Position.X,
                                   datBoneData.Position.Y,
                                   datBoneData.Position.Z)
                         .SetLocalRotationRadians(
                             datBoneData.RotationRadians.X,
                             datBoneData.RotationRadians.Y,
                             datBoneData.RotationRadians.Z)
                         .SetLocalScale(
                             datBoneData.Scale.X,
                             datBoneData.Scale.Y,
                             datBoneData.Scale.Z);

        foreach (var datChildBone in datBone.Children) {
          boneQueue.Enqueue((finBone, datChildBone));
        }
      }

      // Adds vertices
      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();
      foreach (var jObj in dat.RootJObjs) {
        foreach (var dObj in jObj.DObjs) {
          foreach (var pObj in dObj.PObjs) {
            foreach (var datPrimitive in pObj.Primitives) {
              var finVertices =
                  datPrimitive
                      .Vertices
                      .Select(datVertex => {
                        var finVertex = finSkin.AddVertex(datVertex.Position);

                        finVertex.SetLocalNormal(datVertex.Normal);
                        finVertex.SetColor(datVertex.Color);

                        if (datVertex.Uv0 != null) {
                          var uv0 = datVertex.Uv0.Value;
                          finVertex.SetUv(0, uv0.X, uv0.Y);
                        }

                        if (datVertex.Uv1 != null) {
                          var uv1 = datVertex.Uv1.Value;
                          finVertex.SetUv(1, uv1.X, uv1.Y);
                        }

                        return finVertex;
                      })
                      .ToArray();

              switch (datPrimitive.Type) {
                case GxOpcode.DRAW_TRIANGLES: {
                  finMesh.AddTriangles(finVertices);
                  break;
                }
                case GxOpcode.DRAW_QUADS: {
                  finMesh.AddQuads(finVertices);
                  break;
                }
                case GxOpcode.DRAW_TRIANGLE_STRIP: {
                  finMesh.AddTriangleStrip(finVertices);
                  break;
                }
              }
            }
          }
        }
      }

      return finModel;
    }
  }
}