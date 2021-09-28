using System.Collections.Generic;

using Assimp;

using fin.math.matrix;
using fin.model;

namespace fin.exporter.assimp {
  public class AssimpSkeletonBuilder {
    public void BuildAndBindSkeleton(
        Scene assScene,
        IModel finModel) {
      var finRootBone = finModel.Skeleton.Root;

      var assRootNode = assScene.RootNode;

      var boneQueue = new Queue<(Node, IBone)>();
      boneQueue.Enqueue((assRootNode, finRootBone));

      var skinNodesAndBones = new List<(Bone, IBone)>();
      while (boneQueue.Count > 0) {
        var (assNode, finBone) = boneQueue.Dequeue();

        this.ApplyBoneOrientationToNode_(assNode, finBone);

        foreach (var childFinBone in finBone.Children) {
          var childAssNode = new Node(childFinBone.Name);
          assNode.Children.Add(childAssNode);

          boneQueue.Enqueue((childAssNode, childFinBone));
        }
      }
    }

    private void ApplyBoneOrientationToNode_(Node assNode, IBone finBone) {
      var finTransform =
          MatrixTransformUtil.FromTrs(finBone.LocalPosition,
                                      finBone.LocalRotation,
                                      finBone.LocalScale);

      var assNodeTransform = assNode.Transform;
      MatrixConversionUtil.CopyFinIntoAssimp(
          finTransform,
          ref assNodeTransform);
      assNode.Transform = assNodeTransform;
    }
  }
}