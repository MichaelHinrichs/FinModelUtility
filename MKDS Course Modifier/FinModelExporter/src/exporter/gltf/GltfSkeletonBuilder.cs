using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using fin.math;
using fin.model;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  using GltfNode = Node;
  using GltfSkin = Skin;

  public class GltfSkeletonBuilder {
    public (GltfNode, IBone)[] BuildAndBindSkeleton(
        GltfNode rootNode,
        GltfSkin skin,
        ISkeleton skeleton) {
      var rootBone = skeleton.Root;

      var boneQueue = new Queue<(GltfNode, IBone)>();
      boneQueue.Enqueue((rootNode, rootBone));

      var skinNodesAndBones = new List<(GltfNode, IBone)>();
      while (boneQueue.Count > 0) {
        var (node, bone) = boneQueue.Dequeue();

        this.ApplyBoneOrientationToNode_(node, bone);

        if (bone != rootBone) {
          skinNodesAndBones.Add((node, bone));
        }

        foreach (var child in bone.Children) {
          boneQueue.Enqueue((node.CreateNode(child.Name), child));
        }
      }

      skin.BindJoints(skinNodesAndBones
                      .Select(skinNodesAndBone => skinNodesAndBone.Item1)
                      .ToArray());

      return skinNodesAndBones.ToArray();
    }

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