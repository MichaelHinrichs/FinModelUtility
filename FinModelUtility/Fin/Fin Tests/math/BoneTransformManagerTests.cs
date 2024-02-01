using System.Numerics;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

using NUnit.Framework;

namespace fin.math {
  public class BoneTransformManagerTests {
    [Test]
    public void TestWorldSpaceMatrixProjectsPointRelativeToOrigin() {
      var model = new ModelImpl();
      var bone = model.Skeleton.Root.AddChild(1, 2, 3)
                      .SetLocalRotationRadians(1, 2, 3)
                      .SetLocalScale(1, 2, 3);
      var boneWeights = model.Skin.GetOrCreateBoneWeights(VertexSpace.RELATIVE_TO_WORLD, bone);

      var btm = new BoneTransformManager();
      btm.CalculateMatrices(model);

      var position = new Vector3(1, 2, 3);
      btm.ProjectPosition(boneWeights, ref position);

      Asserts.Equal(new Vector3(1, 2, 3), position);
    }

    [Test]
    public void TestBoneSpaceMatrixProjectsPointRelativeToBone() {
      var model = new ModelImpl();
      var bone = model.Skeleton.Root.AddChild(1, 2, 3)
                      .SetLocalRotationRadians(1, 2, 3)
                      .SetLocalScale(1, 2, 3);
      var boneWeights = model.Skin.GetOrCreateBoneWeights(VertexSpace.RELATIVE_TO_BONE, bone);

      var btm = new BoneTransformManager();
      btm.CalculateMatrices(model);

      var position = new Vector3(1, 2, 3);
      btm.ProjectPosition(boneWeights, ref position);

      Asserts.Equal(new Vector3(1, 2, 3), position);
    }
  }
}