using System;
using System.Collections.Generic;
using System.Numerics;

using fin.data;
using fin.math.matrix;
using fin.model;


namespace fin.math {
  public class BoneTransformManager {
    private readonly SoftwareModelViewMatrixTransformer transformer_ = new();

    // TODO: This is going to be slow, can we put this somewhere else for O(1) access?
    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToLocalMatrices_ = new();

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IReadOnlyFinMatrix4x4>
        boneWeightsToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IReadOnlyFinMatrix4x4>
        boneWeightsToNeutralInverseWorldMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IReadOnlyFinMatrix4x4>
        boneWeightsToAdditiveWorldMatrices_ = new();

    public void Clear() {
      this.bonesToLocalMatrices_.Clear();
      this.bonesToWorldMatrices_.Clear();
      this.boneWeightsToWorldMatrices_.Clear();
      this.boneWeightsToNeutralInverseWorldMatrices_.Clear();
      this.boneWeightsToAdditiveWorldMatrices_.Clear();
    }

    public IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IAnimation, float)? animationAndFrame,
        bool useLoopingInterpolation = false
    ) {
      var animation = animationAndFrame?.Item1;
      var frame = animationAndFrame?.Item2;

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

        // The root pose of the bone.
        var boneLocalPosition = bone.LocalPosition;
        var boneLocalRotation = bone.LocalRotation != null
                                    ? QuaternionUtil.Create(bone.LocalRotation)
                                    : (Quaternion?) null;
        var boneLocalScale = bone.LocalScale;

        // The pose of the animation, if available.
        var animationLocalPosition =
            boneTracks?.Positions.IsDefined ?? false
                ? boneTracks?.Positions.GetInterpolatedFrame(
                    (float) frame, null, useLoopingInterpolation)
                : null;
        var animationLocalRotation =
            boneTracks?.Rotations.IsDefined ?? false
                ? boneTracks?.Rotations.GetInterpolatedFrame(
                    (float) frame, null, useLoopingInterpolation)
                : null;
        var animationLocalScale =
            boneTracks?.Scales.IsDefined ?? false
                ? boneTracks?.Scales.GetInterpolatedFrame(
                    (float) frame, null, useLoopingInterpolation)
                : null;

        // Uses the animation pose instead of the root pose when available.
        var localPosition = animationLocalPosition ?? boneLocalPosition;
        var localRotation = animationLocalRotation ?? boneLocalRotation;
        var localScale = animationLocalScale ?? boneLocalScale;

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

      foreach (var boneWeights in boneWeightsList) {
        IReadOnlyFinMatrix4x4 boneWeightMatrix;

        var weights = boneWeights.Weights;
        if (weights.Count == 1) {
          var bone = weights[0].Bone;
          boneWeightMatrix = this.GetWorldMatrix(bone);
        } else {
          var mergedMatrix = new FinMatrix4x4();

          foreach (var weight in weights) {
            var bone = weight.Bone;

            var skinToBoneMatrix = weight.SkinToBone;
            var boneMatrix = this.GetWorldMatrix(bone);

            var skinToWorldMatrix = boneMatrix
                                    .CloneAndMultiply(skinToBoneMatrix)
                                    .MultiplyInPlace(weight.Weight);

            mergedMatrix.AddInPlace(skinToWorldMatrix);
          }

          boneWeightMatrix = mergedMatrix;
        }

        this.boneWeightsToWorldMatrices_[boneWeights] = boneWeightMatrix;
        if (animation == null) {
          this.boneWeightsToNeutralInverseWorldMatrices_[boneWeights] =
              boneWeightMatrix.CloneAndInvert();
          this.boneWeightsToAdditiveWorldMatrices_[boneWeights] =
              new FinMatrix4x4().SetIdentity();
        } else {
          var mergedMatrix = this.boneWeightsToWorldMatrices_[boneWeights];
          var mergedNeutralInverseMatrix =
              this.boneWeightsToNeutralInverseWorldMatrices_[boneWeights];
          this.boneWeightsToAdditiveWorldMatrices_[boneWeights] =
              mergedMatrix.CloneAndMultiply(mergedNeutralInverseMatrix);
        }
      }

      return bonesToIndex;
    }

    public IReadOnlyFinMatrix4x4 GetLocalMatrix(IBone bone)
      => this.bonesToLocalMatrices_[bone];

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    public IReadOnlyFinMatrix4x4? GetTransformMatrix(IVertex vertex,
      bool forcePreproject = false) {
      var weights = vertex.BoneWeights?.Weights;
      var preproject =
          (vertex.PreprojectMode != PreprojectMode.NONE || forcePreproject) &&
          weights?.Count > 0;

      if (!preproject) {
        return null;
      }

      return vertex.PreprojectMode switch {
          // If preproject mode is none, then the vertices are already in the same position as the bones.
          // To calculate the animation, we have to first "undo" the root pose via an inverted matrix. 
          PreprojectMode.NONE => this.boneWeightsToAdditiveWorldMatrices_[
              vertex.BoneWeights!],
          // If preproject mode is bone, then we need to transform the vertex by one or more bones.
          PreprojectMode.BONE => this.boneWeightsToWorldMatrices_[
              vertex.BoneWeights!],
          // If preproject mode is root, then the vertex needs to be transformed relative to
          // some root bone.
          PreprojectMode.ROOT => this.GetWorldMatrix(weights[0].Bone.Root),
          _                   => throw new ArgumentOutOfRangeException()
      };
    }

    public void ProjectVertex(
        IVertex vertex,
        IPosition outPosition,
        INormal? outNormal = null,
        bool forcePreproject = false) {
      var transformMatrix = this.GetTransformMatrix(vertex, forcePreproject);

      var localPosition = vertex.LocalPosition;
      var localNormal = vertex.LocalNormal;
      if (transformMatrix == null) {
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

      this.transformer_.Set(transformMatrix);

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
    }

    public void ProjectVertex(IBone bone,
                              ref double x,
                              ref double y,
                              ref double z) {
      this.transformer_.Set(this.GetWorldMatrix(bone));
      this.transformer_.ProjectVertex(ref x, ref y, ref z);
    }
  }
}