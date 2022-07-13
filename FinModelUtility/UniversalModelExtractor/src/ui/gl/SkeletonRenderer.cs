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

    public SkeletonRenderer(ISkeleton skeleton,
                            BoneTransformManager boneTransformManager) {
      this.Skeleton = skeleton;
      this.boneTransformManager_ = boneTransformManager;
    }

    public ISkeleton Skeleton { get; }

    public void Render() {
      GL.Disable(EnableCap.DepthTest);

      GL.LineWidth(5);

      GL.Begin(PrimitiveType.Lines);
      GL.Color4(1f, 0, 0, 1);

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
          GL.Vertex3(parentX, parentY, parentZ);
          GL.Vertex3(x, y, z);
        }

        var location = (x, y, z);
        foreach (var child in bone.Children) {
          boneQueue.Enqueue((child, location));
        }
      }

      GL.End();

      GL.Color4(1f, 1, 1, 1);
      GL.Enable(EnableCap.DepthTest);
    }
  }
}