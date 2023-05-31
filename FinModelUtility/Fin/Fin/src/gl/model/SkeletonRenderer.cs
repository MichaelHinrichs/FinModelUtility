using fin.math;
using fin.model;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace fin.gl.model {
  public interface ISkeletonRenderer : IRenderable {
    ISkeleton Skeleton { get; }
    IBone? SelectedBone { get; set; }
    float Scale { get; set; }
  }

  /// <summary>
  ///   A renderer for a Fin model's skeleton.
  /// </summary>
  public class SkeletonRenderer : ISkeletonRenderer {
    private readonly IBoneTransformManager boneTransformManager_;

    public SkeletonRenderer(ISkeleton skeleton,
                            IBoneTransformManager boneTransformManager) {
      this.Skeleton = skeleton;
      this.boneTransformManager_ = boneTransformManager;
    }

    public ISkeleton Skeleton { get; }
    public IBone? SelectedBone { get; set; }
    public float Scale { get; set; } = 1;

    public void Render() {
      GlTransform.PassMatricesIntoGl();

      GlUtil.SetDepth(DepthMode.IGNORE_DEPTH_BUFFER, DepthCompareType.Always);

      var rootBone = this.Skeleton.Root;

      // Renders lines from each bone to its parent.
      {
        GL.LineWidth(1);
        GL.Begin(PrimitiveType.Lines);

        GL.Color4(0, 0, 1f, 1);

        var boneQueue = new Queue<(IBone, Vector3?)>();
        boneQueue.Enqueue((this.Skeleton.Root, null));
        while (boneQueue.Any()) {
          var (bone, parentLocation) = boneQueue.Dequeue();

          Vector3? location = null;

          if (bone != rootBone) {
            var xyz = new Vector3();

            this.boneTransformManager_.ProjectPosition(bone, ref xyz);

            if (parentLocation != null) {
              var parentPos = parentLocation.Value;
              GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref parentPos));
              GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref xyz));
            }

            location = xyz;
          }

          foreach (var child in bone.Children) {
            boneQueue.Enqueue((child, location));
          }
        }

        GL.End();
      }

      // Renders lines out from each bone in its direction.
      {
        GL.LineWidth(5);
        GL.Begin(PrimitiveType.Lines);

        GL.Color4(1f, 0, 0, 1);

        foreach (var bone in this.Skeleton) {
          if (bone == rootBone || bone == this.SelectedBone) {
            continue;
          }

          var from = new Vector3();
          this.boneTransformManager_.ProjectPosition(bone, ref from);

          var normal = new Vector3(1, 0, 0);
          this.boneTransformManager_.ProjectNormal(bone, ref normal);

          var normalScale = 50f / this.Scale;
          normal = Vector3.Multiply(normal, normalScale);

          GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref from));
          GL.Vertex3(from.X + normal.X, from.Y + normal.Y, from.Z + normal.Z);
        }

        GL.End();

        if (this.SelectedBone != null) {
          GL.LineWidth(8);
          GL.Begin(PrimitiveType.Lines);

          GL.Color4(1f, 1f, 1f, 1);

          var from = new Vector3();
          this.boneTransformManager_.ProjectPosition(this.SelectedBone, ref from);

          var normal = new Vector3(1, 0, 0);
          this.boneTransformManager_.ProjectNormal(this.SelectedBone, ref normal);

          var normalScale = 50f / this.Scale;
          normal = Vector3.Multiply(normal, normalScale);

          GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref from));
          GL.Vertex3(from.X + normal.X, from.Y + normal.Y, from.Z + normal.Z);

          GL.End();
        }
      }

      // Renders points at the start of each bone.
      {
        GL.PointSize(8);
        GL.Begin(PrimitiveType.Points);

        GL.Color4(1f, 0, 0, 1);

        foreach (var bone in this.Skeleton) {
          if (bone == rootBone || bone == this.SelectedBone) {
            continue;
          }

          var from = new Vector3();
          this.boneTransformManager_.ProjectPosition(bone, ref from);

          GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref from));
        }

        GL.End();

        if (this.SelectedBone != null) {
          GL.PointSize(11);
          GL.Begin(PrimitiveType.Points);

          GL.Color4(1f, 1f, 1f, 1);

          var from = new Vector3();
          this.boneTransformManager_.ProjectPosition(this.SelectedBone, ref from);

          GL.Vertex3(Unsafe.As<Vector3, OpenTK.Vector3>(ref from));

          GL.End();
        }
      }

      GL.Color4(1f, 1, 1, 1);
      GL.Enable(EnableCap.DepthTest);
    }
  }
}