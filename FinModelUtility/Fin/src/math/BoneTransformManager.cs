using System.Collections.Generic;

using fin.math.matrix;
using fin.model;

namespace fin.math {
  public class BoneTransformManager {
    private readonly SoftwareModelViewMatrixTransformer transformer_ = new();

    // TODO: This is going to be slow, can we put this somewhere else for O(1) access?
    private readonly Dictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToLocalMatrices_ = new();

    private readonly Dictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToWorldMatrices_ = new();

    public IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        (IAnimation, float)? animationAndFrame) {
      var animation = animationAndFrame?.Item1;
      var frame = animationAndFrame?.Item2;

      this.transformer_.Push();
      this.transformer_.Identity();

      // TODO: Use a pool of matrices to prevent unneeded instantiations.
      var rootMatrix = new FinMatrix4x4();
      this.transformer_.Get(rootMatrix);

      // TODO: Cache this directly on the bone itself instead.
      var bonesToIndex = new Dictionary<IBone, int>();
      var boneIndex = -1;

      var boneQueue = new Queue<(IBone, IFinMatrix4x4)>();
      boneQueue.Enqueue((rootBone, rootMatrix));
      while (boneQueue.Count > 0) {
        var (bone, matrix) = boneQueue.Dequeue();

        this.transformer_.Set(matrix);

        IBoneTracks? boneTracks = null;
        animation?.BoneTracks.TryGetValue(bone, out boneTracks);

        var localPosition = boneTracks?.Positions.GetInterpolatedFrame(0) ??
                            bone.LocalPosition;

        var localRotation = boneTracks?.Rotations.GetInterpolatedFrame(0) ??
                            (bone.LocalRotation != null
                                 ? QuaternionUtil.Create(bone.LocalRotation)
                                 : null);

        var localScale = boneTracks?.Scales.GetInterpolatedFrame(0) ??
                         bone.LocalScale;

        var localMatrix =
            MatrixTransformUtil.FromTrs(localPosition,
                                        localRotation,
                                        localScale);
        matrix.MultiplyInPlace(localMatrix);

        this.bonesToLocalMatrices_[bone] = localMatrix;
        this.bonesToWorldMatrices_[bone] = matrix;
        bonesToIndex[bone] = boneIndex++;

        foreach (var child in bone.Children) {
          // TODO: Use a pool of matrices to prevent unneeded instantiations.
          boneQueue.Enqueue((child, matrix.Clone()));
        }
      }

      this.transformer_.Pop();

      return bonesToIndex;
    }

    public IReadOnlyFinMatrix4x4 GetLocalMatrix(IBone bone)
      => this.bonesToLocalMatrices_[bone];

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    public void ProjectVertex(
        IVertex vertex,
        IPosition outPosition,
        INormal? outNormal = null) {
      var preproject = vertex.Preproject && vertex.Weights?.Count > 0;

      var localPosition = vertex.LocalPosition;
      var localNormal = vertex.LocalNormal;
      if (!preproject) {
        outPosition.X = localPosition.X;
        outPosition.Y = localPosition.Y;
        outPosition.Z = localPosition.Z;

        if (outNormal != null && localNormal != null) {
          outNormal.X = localNormal.X;
          outNormal.Y = localNormal.Y;
          outNormal.Z = localNormal.Z;
        }
        return;
      }

      // TODO: Precompute these in a shared way somehow.
      var mergedMatrix = new FinMatrix4x4();
      foreach (var weight in vertex.Weights) {
        var skinToBoneMatrix = weight.SkinToBone;
        var boneMatrix = this.GetWorldMatrix(weight.Bone);

        var skinToWorldMatrix = boneMatrix.CloneAndMultiply(skinToBoneMatrix)
                                          .MultiplyInPlace(weight.Weight);

        mergedMatrix.AddInPlace(skinToWorldMatrix);
      }

      this.transformer_.Push();
      this.transformer_.Set(mergedMatrix);

      double x = localPosition.X;
      double y = localPosition.Y;
      double z = localPosition.Z;
      this.transformer_.ProjectVertex(ref x, ref y, ref z);
      outPosition.X = (float) x;
      outPosition.Y = (float) y;
      outPosition.Z = (float) z;

      if (outNormal != null && localNormal != null) {
        double nX = localNormal.X;
        double nY = localNormal.Y;
        double nZ = localNormal.Z;
        this.transformer_.ProjectNormal(ref nX, ref nY, ref nZ);

        // All of the normals are inside-out for some reason, we have to flip
        // them manually.
        outNormal.X = (float) nX;
        outNormal.Y = (float) nY;
        outNormal.Z = (float) nZ;
      }

      this.transformer_.Pop();
    }
  }
}