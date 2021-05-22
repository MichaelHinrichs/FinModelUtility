using System.Collections.Generic;
using System.Numerics;

using fin.math;
using fin.model;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  public class GltfBoneGatherer : IExporter {
    public void GatherBones(
        Scene scene,
        Skin skin,
        ISkeleton skeleton,
        IAnimations animationManager) {
      var rootBone = skeleton.Root;

      var animations = animationManager.Animations;
      var firstAnimation = animations.Count > 0 ? animations[0] : null;

      var skinNodes = new List<Node>();

      var boneQueue = new Queue<(Node, IBone)>();
      boneQueue.Enqueue((scene.CreateNode(), rootBone));
      while (boneQueue.Count > 0) {
        var (parentNode, bone) = boneQueue.Dequeue();
        var boneName = bone.Name;

        var node = parentNode.CreateNode(boneName);

        // We should be able to use the raw bone positions, but this screws up
        // bones with multiple weights for some reason, perhaps because the
        // model is contorted in an unnatural way? Anyway, we NEED to use the
        // first animation instead.
        if (firstAnimation != null) {
          this.ApplyFirstFrameToNode_(node, firstAnimation.BoneTracks[bone]);
        } else {
          this.ApplyBoneOrientationToNode_(node, bone);
        }

        if (bone != rootBone) {
          skinNodes.Add(node);
        }
      }
      skin.BindJoints(skinNodes.ToArray());
    }

    /// <summary>
    ///   It seems like some animations shrink a bone to 0 to hide it, but this
    ///   prevents us from calculating a determinant to invert the matrix. As a
    ///   result, we can't safely include the scale here.
    /// </summary>
    private void ApplyFirstFrameToNode_(Node node, IBoneTracks? boneTracks)
      => this.ApplyOrientationToNode_(
          node,
          boneTracks?.Positions?.GetInterpolatedAtFrame(0),
          boneTracks?.Rotations?.GetInterpolatedAtFrame(0),
          null);

    private void ApplyBoneOrientationToNode_(Node node, IBone bone)
      => this.ApplyOrientationToNode_(
          node,
          bone.LocalPosition,
          bone.LocalRotation,
          bone.LocalScale);

    private void ApplyOrientationToNode_(
        Node node,
        IPosition? position,
        IQuaternion? rotation,
        IScale? scale) {
      if (position != null) {
        node.WithLocalTranslation(
            new Vector3(position.X, position.Y, position.Z));
      }

      if (rotation is {Length: > 0}) {
        node.WithLocalRotation(QuaternionUtil.Create(
                                   rotation.XRadians,
                                   rotation.YRadians,
                                   rotation.ZRadians));
      }

      if (scale != null) {
        node.WithLocalScale(new Vector3(scale.X, scale.Y, scale.Z));
      }
    }
  }
}