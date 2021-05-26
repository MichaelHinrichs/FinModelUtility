using System.Collections.Generic;
using System.Numerics;

using fin.math;
using fin.model;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  using GltfNode = Node;
  using GltfSkin = Skin;

  public class GltfSkeletonBuilder {
    public void BuildAndBindSkeleton(
        GltfNode rootNode,
        GltfSkin skin,
        ISkeleton skeleton,
        IAnimation? firstAnimation) {
      var rootBone = skeleton.Root;

      var boneQueue = new Queue<(GltfNode, IBone)>();
      boneQueue.Enqueue((rootNode, rootBone));

      var skinNodes = new List<GltfNode>();
      while (boneQueue.Count > 0) {
        var (node, bone) = boneQueue.Dequeue();

        // We should be able to use the raw bone positions, but this screws up
        // bones with multiple weights for some reason, perhaps because the
        // model is contorted in an unnatural way? Anyway, we NEED to use the
        // first animation instead.
        if (firstAnimation != null) {
          firstAnimation.BoneTracks.TryGetValue(bone, out var boneTracks);
          this.ApplyFirstFrameToNode_(node, boneTracks);
        } else {
          this.ApplyBoneOrientationToNode_(node, bone);
        }

        if (bone != rootBone) {
          skinNodes.Add(node);
        }

        foreach (var child in bone.Children) {
          boneQueue.Enqueue((node.CreateNode(child.Name), child));
        }
      }
      skin.BindJoints(skinNodes.ToArray());
    }

    /// <summary>
    ///   It seems like some animations shrink a bone to 0 to hide it, but this
    ///   prevents us from calculating a determinant to invert the matrix. As a
    ///   result, we can't safely include the scale here.
    /// </summary>
    private void ApplyFirstFrameToNode_(GltfNode node, IBoneTracks? boneTracks)
      => this.ApplyOrientationToNode_(
          node,
          boneTracks?.Positions.GetInterpolatedAtFrame(0),
          boneTracks?.Rotations.GetInterpolatedAtFrame(0),
          null);

    private void ApplyBoneOrientationToNode_(GltfNode node, IBone bone)
      => this.ApplyOrientationToNode_(
          node,
          bone.LocalPosition,
          bone.LocalRotation != null
              ? QuaternionUtil.Create(
                  bone.LocalRotation.XRadians,
                  bone.LocalRotation.YRadians,
                  bone.LocalRotation.ZRadians)
              : null,
          bone.LocalScale);

    private void ApplyOrientationToNode_(
        GltfNode node,
        IPosition? position,
        Quaternion? rotation,
        IScale? scale) {
      if (position != null) {
        node.WithLocalTranslation(
            new Vector3(position.X, position.Y, position.Z));
      }

      if (rotation?.Length() > 0) {
        node.WithLocalRotation(rotation.Value);
      }

      if (scale != null) {
        node.WithLocalScale(new Vector3(scale.X, scale.Y, scale.Z));
      }
    }
  }
}