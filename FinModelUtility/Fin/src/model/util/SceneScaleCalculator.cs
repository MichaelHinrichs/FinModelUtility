using System;
using System.Collections.Generic;

using fin.model.impl;
using fin.scene;

using System.Numerics;

namespace fin.model.util {
  public static class SceneScaleCalculator {
    public record Bounds(float MinX,
                         float MinY,
                         float MinZ,
                         float MaxX,
                         float MaxY,
                         float MaxZ);

    public static float CalculateScale(IScene scene) {
      var bounds = CalculateBounds(scene);
      return MathF.Sqrt(MathF.Pow(bounds.MaxX - bounds.MinX, 2) +
                        MathF.Pow(bounds.MaxY - bounds.MinY, 2) +
                        MathF.Pow(bounds.MaxZ - bounds.MinZ, 2));
    }

    public static Bounds CalculateBounds(IScene scene) {
      var minX = float.MaxValue;
      var minY = float.MaxValue;
      var minZ = float.MaxValue;
      var maxX = float.MinValue;
      var maxY = float.MinValue;
      var maxZ = float.MinValue;

      var position = new ModelImpl.PositionImpl();

      foreach (var area in scene.Areas) {
        foreach (var obj in area.Objects) {
          foreach (var sceneModel in obj.Models) {
            var model = sceneModel.Model;
            var boneTransformManager = sceneModel.BoneTransformManager;

            var anyVertices = false;
            foreach (var vertex in model.Skin.Vertices) {
              anyVertices = true;

              boneTransformManager.ProjectVertex(
                  vertex,
                  out var x,
                  out var y,
                  out var z);

              minX = MathF.Min(minX, x);
              maxX = MathF.Max(maxX, x);

              minY = MathF.Min(minY, y);
              maxY = MathF.Max(maxY, y);

              minZ = MathF.Min(minZ, z);
              maxZ = MathF.Max(maxZ, z);
            }

            if (!anyVertices) {
              var boneQueue = new Queue<IBone>();
              boneQueue.Enqueue(model.Skeleton.Root);

              while (boneQueue.Count > 0) {
                var bone = boneQueue.Dequeue();

                var xyz = new Vector3();

                boneTransformManager.ProjectPosition(bone, ref xyz);

                minX = MathF.Min(minX, xyz.X);
                maxX = MathF.Max(maxX, xyz.X);

                minY = MathF.Min(minY, xyz.Y);
                maxY = MathF.Max(maxY, xyz.Y);

                minZ = MathF.Min(minZ, xyz.Z);
                maxZ = MathF.Max(maxZ, xyz.Z);

                foreach (var child in bone.Children) {
                  boneQueue.Enqueue(child);
                }
              }
            }
          }
        }
      }

      return new Bounds(minX, minY, minZ, maxX, maxY, maxZ);
    }
  }
}