using fin.math;
using fin.model;
using OpenTK.Graphics.OpenGL;

using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model's skeleton.
  /// </summary>
  public class SkeletonRenderer {
    private readonly BoneTransformManager boneTransformManager_;
    private readonly float scale_;

    public SkeletonRenderer(ISkeleton skeleton,
                            BoneTransformManager boneTransformManager,
                            float scale) {
      this.Skeleton = skeleton;
      this.boneTransformManager_ = boneTransformManager;
      this.scale_ = scale;
    }

    public ISkeleton Skeleton { get; }

    public IBone? SelectedBone { get; set; }

    public void Render() {
      GL.Disable(EnableCap.DepthTest);

      {
        // Renders lines from each bone to its parent.
        {
          GL.LineWidth(1);
          GL.Begin(PrimitiveType.Lines);

          GL.Color4(0, 0, 1f, 1);

          var boneQueue = new Queue<(IBone, (double, double, double)?)>();
          boneQueue.Enqueue((this.Skeleton.Root, null));
          while (boneQueue.Any()) {
            var (bone, parentLocation) = boneQueue.Dequeue();

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

            var location = (x, y, z);
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

          foreach (var bone in this.Skeleton) {
            if (bone == this.SelectedBone) {
              GL.Color4(1f, 1f, 1f, 1);
            } else {
              GL.Color4(1f, 0, 0, 1);
            }

            var fromX = 0f;
            var fromY = 0f;
            var fromZ = 0f;
            this.boneTransformManager_.ProjectVertex(
              bone, ref fromX, ref fromY, ref fromZ);

            var toX = 50f / this.scale_;
            var toY = 0f;
            var toZ = 0f;
            this.boneTransformManager_.ProjectVertex(
              bone, ref toX, ref toY, ref toZ);

            GL.Vertex3(fromX, fromY, fromZ);
            GL.Vertex3(toX, toY, toZ);
          }

          GL.End();
        }
      }

      // Renders points at the start of each bone.
      {
        GL.PointSize(8);
        GL.Begin(PrimitiveType.Points);

        foreach (var bone in this.Skeleton) {
          if (bone == this.SelectedBone) {
            GL.Color4(1f, 1f, 1f, 1);
          } else {
            GL.Color4(1f, 0, 0, 1);
          }

          var fromX = 0f;
          var fromY = 0f;
          var fromZ = 0f;
          this.boneTransformManager_.ProjectVertex(
            bone, ref fromX, ref fromY, ref fromZ);

          GL.Vertex3(fromX, fromY, fromZ);
        }

        GL.End();
      }


      GL.Color4(1f, 1, 1, 1);
      GL.Enable(EnableCap.DepthTest);
    }
  }
}