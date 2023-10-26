using System.Runtime.InteropServices.ComTypes;

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
      var finBoneByJObj = new Dictionary<JObj, IBone>();
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
        finBoneByJObj[datBone] = finBone;

        foreach (var datChildBone in datBone.Children) {
          boneQueue.Enqueue((finBone, datChildBone));
        }
      }

      // Adds mesh and materials
      var finMaterialManager = finModel.MaterialManager;
      var finMaterialsByMObjOffset = new Dictionary<uint, IMaterial>();
      var finTexturesByTObjOffset = new Dictionary<uint, ITexture>();

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();
      foreach (var jObj in dat.JObjs) {
        foreach (var dObj in jObj.DObjs) {
          // Gets material
          IMaterial? finMaterial = null;
          var mObj = dObj.MObj;
          var mObjOffset = dObj.Header.MObjOffset;
          if (mObj != null && !finMaterialsByMObjOffset.TryGetValue(
                  mObjOffset,
                  out finMaterial)) {
            var tObj = mObj.TObj;
            if (tObj == null) {
              finMaterial = finMaterialManager.AddNullMaterial();
            } else {
              var tObjOffset = mObj.TObjOffset;
              if (!finTexturesByTObjOffset.TryGetValue(
                      tObjOffset,
                      out var finTexture)) {
                finTexture = finMaterialManager.CreateTexture(tObj.Image);
                finTexturesByTObjOffset[tObjOffset] = finTexture;
              }

              finMaterial = finMaterialManager.AddTextureMaterial(finTexture);
            }

            finMaterialsByMObjOffset[mObjOffset] = finMaterial;
          }

          // Adds polygons
          foreach (var pObj in dObj.PObjs) {
            var vertexSpace = pObj.VertexSpace;
            var finWeights =
                pObj.Weights
                    ?.Select(pObjWeights => finSkin.GetOrCreateBoneWeights(
                                 vertexSpace,
                                 pObjWeights
                                     .Select(
                                         pObjWeight => new BoneWeight(
                                             finBoneByJObj[pObjWeight.JObj],
                                             null,
                                             pObjWeight.Weight
                                         ))
                                     .ToArray()))
                    .ToArray();

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

                        if (datVertex.WeightId != null && finWeights != null) {
                          finVertex.SetBoneWeights(
                              finWeights[datVertex.WeightId.Value]);
                        }

                        return finVertex;
                      })
                      .ToArray();

              var finPrimitive = datPrimitive.Type switch {
                  GxOpcode.DRAW_TRIANGLES => finMesh.AddTriangles(finVertices),
                  GxOpcode.DRAW_QUADS     => finMesh.AddQuads(finVertices),
                  GxOpcode.DRAW_TRIANGLE_STRIP => finMesh.AddTriangleStrip(
                      finVertices)
              };

              if (finMaterial != null) {
                finPrimitive.SetMaterial(finMaterial);
              }
            }
          }
        }
      }

      return finModel;
    }
  }
}