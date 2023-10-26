using System.Linq;
using System.Runtime.InteropServices.ComTypes;

using Assimp;
using Assimp.Unmanaged;

using dat.schema;

using fin.io;
using fin.model;
using fin.model.impl;
using fin.model.io.importers;
using fin.util.hex;

using gx;

using schema.binary;

namespace dat.api {
  public class DatModelImporter : IModelImporter<DatModelFileBundle> {
    public IModel ImportModel(DatModelFileBundle modelFileBundle) {
      var dat =
          modelFileBundle.PrimaryDatFile.ReadNew<Dat>(Endianness.BigEndian);

      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;

      // Adds skeleton
      var finBoneByJObj = new Dictionary<JObj, IBone>();
      var boneWeightsByJObj = new Dictionary<JObj, IBoneWeights>();
      var boneQueue = new Queue<(IBone finParentBone, JObj datBone)>();
      foreach (var datRootBone in dat.RootJObjs) {
        boneQueue.Enqueue((finModel.Skeleton.Root, datRootBone));
      }

      while (boneQueue.Count > 0) {
        var (finParentBone, jObj) = boneQueue.Dequeue();

        var datBoneData = jObj.Data;

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
        finBone.Name = jObj.Name;

        finBoneByJObj[jObj] = finBone;
        boneWeightsByJObj[jObj] =
            finSkin.GetOrCreateBoneWeights(VertexSpace.BONE, finBone);

        foreach (var datChildBone in jObj.Children) {
          boneQueue.Enqueue((finBone, datChildBone));
        }
      }

      // Adds mesh and materials
      var finMaterialManager = finModel.MaterialManager;
      var finMaterialsByMObjOffset = new Dictionary<uint, IMaterial>();
      var finTexturesByTObjOffset = new Dictionary<uint, ITexture>();

      var finMesh = finSkin.AddMesh();
      foreach (var jObj in dat.JObjs) {
        var defaultBoneWeights = boneWeightsByJObj[jObj];

        foreach (var dObj in jObj.DObjs) {
          // Gets material
          IMaterial? finMaterial = null;
          var mObj = dObj.MObj;
          var mObjOffset = dObj.Header.MObjOffset;
          if (mObj != null && !finMaterialsByMObjOffset.TryGetValue(
                  mObjOffset,
                  out finMaterial)) {
            var tObjsAndOffsets = mObj.TObjsAndOffsets.ToArray();
            if (tObjsAndOffsets.Length == 0) {
              finMaterial = finMaterialManager.AddNullMaterial();
            } else {
              ITexture? firstTexture = null;
              foreach (var (tObjOffset, tObj) in tObjsAndOffsets) {
                if (!finTexturesByTObjOffset.TryGetValue(
                        tObjOffset,
                        out var finTexture)) {
                  finTexture = finMaterialManager.CreateTexture(tObj.Image);
                  finTexture.Name = tObj.Name ?? tObjOffset.ToHex();

                  finTexturesByTObjOffset[tObjOffset] = finTexture;
                }

                if (firstTexture == null) {
                  firstTexture = finTexture;
                }
              }

              finMaterial =
                  finMaterialManager.AddTextureMaterial(firstTexture!);
            }

            finMaterial.Name = mObj.Name ?? mObjOffset.ToHex();

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
                                             pObjWeight.JObj.InverseBindMatrix,
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

                        // TODO: Is this right???
                        if (datVertex.WeightId != null) {
                          if (finWeights != null) {
                            finVertex.SetBoneWeights(
                                finWeights[datVertex.WeightId.Value]);
                          }
                        } else {
                          finVertex.SetBoneWeights(defaultBoneWeights);
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