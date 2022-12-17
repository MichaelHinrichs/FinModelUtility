using fin.math;
using fin.model;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
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
      GL.Disable(EnableCap.DepthTest);

      var rootBone = this.Skeleton.Root;

      // Renders lines from each bone to its parent.
      {
        GL.LineWidth(1);
        GL.Begin(PrimitiveType.Lines);

        GL.Color4(0, 0, 1f, 1);

        var boneQueue = new Queue<(IBone, (double, double, double)?)>();
        boneQueue.Enqueue((this.Skeleton.Root, null));
        while (boneQueue.Any()) {
          var (bone, parentLocation) = boneQueue.Dequeue();

          (float, float, float)? location = null;

          if (bone != rootBone) {
            var x = 0f;
            var y = 0f;
            var z = 0f;

            this.boneTransformManager_.ProjectVertex(
                bone, ref x, ref y, ref z);

            if (parentLocation != null) {
              var (parentX, parentY, parentZ) = parentLocation.Value;
              GL.Vertex3(parentX, parentY, parentZ);
              GL.Vertex3(x, y, z);
            }

            location = (x, y, z);
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

          var fromX = 0f;
          var fromY = 0f;
          var fromZ = 0f;
          this.boneTransformManager_.ProjectVertex(
              bone, ref fromX, ref fromY, ref fromZ);

          var normalX = 1f;
          var normalY = 0f;
          var normalZ = 0f;
          this.boneTransformManager_.ProjectNormal(
              bone, ref normalX, ref normalY, ref normalZ);

          var normalScale = 50f / this.Scale;
          normalX *= normalScale;
          normalY *= normalScale;
          normalZ *= normalScale;

          GL.Vertex3(fromX, fromY, fromZ);
          GL.Vertex3(fromX + normalX, fromY + normalY, fromZ + normalZ);
        }

        GL.End();

        if (this.SelectedBone != null) {
          GL.LineWidth(8);
          GL.Begin(PrimitiveType.Lines);

          GL.Color4(1f, 1f, 1f, 1);

          var fromX = 0f;
          var fromY = 0f;
          var fromZ = 0f;
          this.boneTransformManager_.ProjectVertex(
              this.SelectedBone, ref fromX, ref fromY, ref fromZ);

          var normalX = 1f;
          var normalY = 0f;
          var normalZ = 0f;
          this.boneTransformManager_.ProjectNormal(
              this.SelectedBone, ref normalX, ref normalY, ref normalZ);

          var normalScale = 50f / this.Scale;
          normalX *= normalScale;
          normalY *= normalScale;
          normalZ *= normalScale;

          GL.Vertex3(fromX, fromY, fromZ);
          GL.Vertex3(fromX + normalX, fromY + normalY, fromZ + normalZ);

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

          var fromX = 0f;
          var fromY = 0f;
          var fromZ = 0f;
          this.boneTransformManager_.ProjectVertex(
              bone, ref fromX, ref fromY, ref fromZ);

          GL.Vertex3(fromX, fromY, fromZ);
        }

        GL.End();

        if (this.SelectedBone != null) {
          GL.PointSize(11);
          GL.Begin(PrimitiveType.Points);

          GL.Color4(1f, 1f, 1f, 1);

          var fromX = 0f;
          var fromY = 0f;
          var fromZ = 0f;
          this.boneTransformManager_.ProjectVertex(
              this.SelectedBone, ref fromX, ref fromY, ref fromZ);

          GL.Vertex3(fromX, fromY, fromZ);

          GL.End();
        }
      }

      GL.Color4(1f, 1, 1, 1);
      GL.Enable(EnableCap.DepthTest);
    }
  }
}