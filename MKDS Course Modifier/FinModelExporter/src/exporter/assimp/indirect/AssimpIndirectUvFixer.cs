using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;

using fin.math;
using fin.model;
using fin.model.impl;

namespace fin.src.exporter.assimp.indirect {
  public class AssimpIndirectUvFixer {
    public void Fix(IModel model, Scene sc) {
      // Fix the UVs.
      var finVertices = model.Skin.Vertices;
      var assMeshes = sc.Meshes;

      var animations = model.AnimationManager.Animations;
      var firstAnimation =
          (animations?.Count ?? 0) > 0 ? animations[0] : null;

      var boneTransformManager = new BoneTransformManager();
      boneTransformManager.CalculateMatrices(
          model.Skeleton.Root,
          firstAnimation != null ? (firstAnimation, 0) : null);
      boneTransformManager.CalculateMatrices(model.Skeleton.Root, null);

      var finVerticesByMaterial =
          model.Skin.Primitives
               .GroupBy(primitive => primitive.Material)
               .Select(grouping => {
                 var positionsAndNormals = grouping
                                           .SelectMany(
                                               primitive
                                                   => primitive.Vertices)
                                           .ToHashSet()
                                           .Select(finVertex => {
                                             IPosition position =
                                                 new ModelImpl.PositionImpl();
                                             INormal normal =
                                                 new ModelImpl.NormalImpl();
                                             boneTransformManager
                                                 .ProjectVertex(
                                                     finVertex,
                                                     position,
                                                     normal);
                                             return (position, normal);
                                           });

                   //var uniquePositions = positionsAndNormals.

                   return positionsAndNormals;
               })
               .ToList();

      var worldFinVerticesAndNormals =
          finVertices.Select(finVertex => {
            IPosition position =
                  new ModelImpl.PositionImpl();
            INormal normal = new ModelImpl.NormalImpl();
            boneTransformManager.ProjectVertex(
                  finVertex,
                  position,
                  normal);
            return (position, normal);
          })
                     .ToArray();

      foreach (var assMesh in assMeshes) {
        var assLocations = assMesh.Vertices;
        var assNormals = assMesh.Normals;

        var assUvs = assMesh.TextureCoordinateChannels;

        var oldAssUvs = new List<Vector3D>[8];
        for (var t = 0; t < 8; ++t) {
          oldAssUvs[t] = assUvs[t].Select(x => x).ToList();
          assUvs[t].Clear();
        }


        var hadUv = new bool[8];
        for (var i = 0; i < assLocations.Count; ++i) {
          var assLocation = assLocations[i];
          var assNormal = assNormals[i];

          // TODO: How to find this more efficiently???
          var minIndex = -1;
          for (var v = 0; v < worldFinVerticesAndNormals.Length; ++v) {
            var (position, normal) = worldFinVerticesAndNormals[v];
            var inNormal = finVertices[v].LocalNormal;

            var tolerance = .005;

            if (i == 131) {
              var finUv = finVertices[v].GetUv(0);

              if (Math.Abs(finUv.U - .9375) < tolerance ||
                  Math.Abs((1 - finUv.U) - .9375) < tolerance) {
                var u0 = finUv.U;
                var v0 = 1 - finUv.V;
                ;
              }
            }

            if (Math.Abs(position.X - assLocation.X) < tolerance &&
                Math.Abs(position.Y - assLocation.Y) < tolerance &&
                Math.Abs(position.Z - assLocation.Z) < tolerance) {
              if (Math.Abs(normal.X - assNormal.X) < tolerance &&
                  Math.Abs(normal.Y - assNormal.Y) < tolerance &&
                  Math.Abs(normal.Z - assNormal.Z) < tolerance) {
                minIndex = v;
                break;
              } else {
                ;
              }
            }
          }

          var minLocation = worldFinVerticesAndNormals[minIndex];
          var minVertex = finVertices[minIndex];

          for (var t = 0; t < 8; ++t) {
            var uv = minVertex.GetUv(t);

            if (uv != null) {
              hadUv[t] = true;
              assUvs[t].Add(new Vector3D(uv.U, 1 - uv.V, 0));
            } else {
              assUvs[t].Add(default);
            }
          }
        }

        for (var t = 0; t < 8; ++t) {
          if (!hadUv[t]) {
            assUvs[t].Clear();
          }
        }

        var lhs = oldAssUvs[0];
        var rhs = assUvs[0];
        for (var i = 0; i < lhs.Count; ++i) {
          var oldAssUv = lhs[i];
          var assUv = rhs[i];

          if (Math.Abs(oldAssUv.X - assUv.X) > .01 ||
              Math.Abs(oldAssUv.Y - assUv.Y) > .01) {
            var assWorld = assLocations[i];
            ;
          }
        }

        ;
      }
    }
  }
}
