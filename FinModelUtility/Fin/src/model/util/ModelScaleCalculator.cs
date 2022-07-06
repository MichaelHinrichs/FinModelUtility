using System;
using System.Collections.Generic;

using Assimp.Configs;

using fin.math;
using fin.model.impl;


namespace fin.model.util {
  public static class ModelScaleCalculator {
    public record Bounds(float MinX,
                         float MinY,
                         float MinZ,
                         float MaxX,
                         float MaxY,
                         float MaxZ);

    public static float CalculateScale(IModel model,
                                       BoneTransformManager
                                           boneTransformManager) {
      var bounds = CalculateBounds(model, boneTransformManager);
      return MathF.Sqrt(MathF.Pow(bounds.MaxX - bounds.MinX, 2) +
                        MathF.Pow(bounds.MaxY - bounds.MinY, 2) +
                        MathF.Pow(bounds.MaxZ - bounds.MinZ, 2));
    }

    public static Bounds CalculateBounds(IModel model,
                                         BoneTransformManager
                                             boneTransformManager) {
      var minX = float.MaxValue;
      var minY = float.MaxValue;
      var minZ = float.MaxValue;
      var maxX = float.MinValue;
      var maxY = float.MinValue;
      var maxZ = float.MinValue;

      var position = new ModelImpl.PositionImpl();

      var anyVertices = false;
      foreach (var mesh in model.Skin.Meshes) {
        foreach (var primitive in mesh.Primitives) {
          foreach (var vertex in primitive.Vertices) {
            anyVertices = true;

            boneTransformManager.ProjectVertex(vertex, position);

            var x = position.X;
            var y = position.Y;
            var z = position.Z;

            minX = MathF.Min(minX, x);
            maxX = MathF.Max(maxX, x);

            minY = MathF.Min(minY, y);
            maxY = MathF.Max(maxY, y);

            minZ = MathF.Min(minZ, z);
            maxZ = MathF.Max(maxZ, z);
          }
        }
      }

      if (!anyVertices) {
        var boneQueue = new Queue<IBone>();
        boneQueue.Enqueue(model.Skeleton.Root);

        while (boneQueue.Count > 0) {
          var bone = boneQueue.Dequeue();

          var x = 0d;
          var y = 0d;
          var z = 0d;

          boneTransformManager.ProjectVertex(bone, ref x, ref y, ref z);

          minX = MathF.Min(minX, (float) x);
          maxX = MathF.Max(maxX, (float) x);

          minY = MathF.Min(minY, (float) y);
          maxY = MathF.Max(maxY, (float) y);

          minZ = MathF.Min(minZ, (float) z);
          maxZ = MathF.Max(maxZ, (float) z);

          foreach (var child in bone.Children) {
            boneQueue.Enqueue(child);
          }
        }
      }

      return new Bounds(minX, minY, minZ, maxX, maxY, maxZ);
    }
  }
}