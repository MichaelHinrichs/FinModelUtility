using fin.math;
using fin.model;

using Tao.OpenGl;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model's skeleton.
  /// </summary>
  public class SkeletonRenderer {
    private readonly BoneTransformManager boneTransformManager_;

    public SkeletonRenderer(ISkeleton skeleton,
                            BoneTransformManager boneTransformManager) {
      this.Skeleton = skeleton;
      this.boneTransformManager_ = boneTransformManager;
    }

    public ISkeleton Skeleton { get; }

    public void Render() {
      Gl.glDisable(Gl.GL_DEPTH_TEST);

      Gl.glLineWidth(5);

      Gl.glBegin(Gl.GL_LINES);
      Gl.glColor4f(1, 0, 0, 1);

      var boneQueue = new Queue<(IBone, (double, double, double)?)>();
      boneQueue.Enqueue((this.Skeleton.Root, null));
      while (boneQueue.Any()) {
        var (bone, parentLocation) = boneQueue.Dequeue();

        var x = 0d;
        var y = 0d;
        var z = 0d;

        this.boneTransformManager_.ProjectVertex(
            bone, ref x, ref y, ref z);

        if (parentLocation != null) {
          var (parentX, parentY, parentZ) = parentLocation.Value;
          Gl.glVertex3d(parentX, parentY, parentZ);
          Gl.glVertex3d(x, y, z);
        }

        var location = (x, y, z);
        foreach (var child in bone.Children) {
          boneQueue.Enqueue((child, location));
        }
      }

      Gl.glEnd();

      Gl.glColor4f(1, 1, 1, 1);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
    }
  }
}