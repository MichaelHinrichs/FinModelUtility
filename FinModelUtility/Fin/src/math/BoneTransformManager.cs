using System;
using System.Collections.Generic;
using System.Numerics;

using fin.data;
using fin.math.matrix;
using fin.model;
using fin.util.asserts;


namespace fin.math {
  public class BoneTransformManager {
    // TODO: This is going to be slow, can we put this somewhere else for O(1) access?
    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToLocalMatrices_ = new();

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToInverseBindMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IReadOnlyFinMatrix4x4>
        boneWeightsToWorldMatrices_ = new();

    public void Clear() {
      this.bonesToLocalMatrices_.Clear();
      this.bonesToWorldMatrices_.Clear();
      this.bonesToInverseBindMatrices_.Clear();
      this.boneWeightsToWorldMatrices_.Clear();
    }

    public IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IAnimation, float)? animationAndFrame,
        bool useLoopingInterpolation = false
    ) {
      var isFirstPass = animationAndFrame == null;

      var animation = animationAndFrame?.Item1;
      var frame = animationAndFrame?.Item2;

      var transformer = new SoftwareModelViewMatrixTransformer();

      // TODO: Use a pool of matrices to prevent unneeded instantiations.
      var rootMatrix = new FinMatrix4x4();
      transformer.Get(rootMatrix);

      // TODO: Cache this directly on the bone itself instead.
      var bonesToIndex = new Dictionary<IBone, int>();
      var boneIndex = -1;

      var boneQueue = new Queue<(IBone, IFinMatrix4x4)>();
      boneQueue.Enqueue((rootBone, rootMatrix));
      while (boneQueue.Count > 0) {
        var (bone, matrix) = boneQueue.Dequeue();

        transformer.Set(matrix);

        // The root pose of the bone.
        var boneLocalPosition = bone.LocalPosition;
        var boneLocalRotation = bone.LocalRotation != null
                                    ? QuaternionUtil.Create(bone.LocalRotation)
                                    : (Quaternion?) null;
        var boneLocalScale = bone.LocalScale;

        // The pose of the animation, if available.
        IBoneTracks? boneTracks = null;
        animation?.BoneTracks.TryGetValue(bone, out boneTracks);
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
        if (isFirstPass) {
          this.bonesToInverseBindMatrices_[bone] = matrix.CloneAndInvert();
        }
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
          var weight = weights[0];
          var bone = weight.Bone;
          
          var skinToBoneMatrix = weight.SkinToBone ?? this.bonesToInverseBindMatrices_[bone];
          var boneMatrix = this.GetWorldMatrix(bone);

          boneWeightMatrix = boneMatrix.CloneAndMultiply(skinToBoneMatrix);
        } else {
          var mergedMatrix = new FinMatrix4x4();

          foreach (var weight in weights) {
            var bone = weight.Bone;

            var skinToBoneMatrix = weight.SkinToBone ?? this.bonesToInverseBindMatrices_[bone];
            var boneMatrix = this.GetWorldMatrix(bone);

            var skinToWorldMatrix = boneMatrix
                                    .CloneAndMultiply(skinToBoneMatrix)
                                    .MultiplyInPlace(weight.Weight);

            mergedMatrix.AddInPlace(skinToWorldMatrix);
          }

          boneWeightMatrix = mergedMatrix;
        }

        this.boneWeightsToWorldMatrices_[boneWeights] = boneWeightMatrix;
      }

      return bonesToIndex;
    }

    public IReadOnlyFinMatrix4x4 GetLocalMatrix(IBone bone)
      => this.bonesToLocalMatrices_[bone];

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    public IReadOnlyFinMatrix4x4? GetTransformMatrix(IVertex vertex,
      bool forcePreproject = false) {

      var boneWeights = vertex.BoneWeights;
      var weights = vertex.BoneWeights?.Weights;
      var preproject =
          (boneWeights?.PreprojectMode != PreprojectMode.NONE || forcePreproject) &&
          weights?.Count > 0;

      if (!preproject) {
        return null;
      }

      var transformMatrix = boneWeights.PreprojectMode switch {
          // If preproject mode is none, then the vertices are already in the same position as the bones.
          // To calculate the animation, we have to first "undo" the root pose via an inverted matrix. 
          PreprojectMode.NONE => this.boneWeightsToWorldMatrices_[
              vertex.BoneWeights!],
          // If preproject mode is bone, then we need to transform the vertex by one or more bones.
          PreprojectMode.BONE => this.boneWeightsToWorldMatrices_[
              vertex.BoneWeights!],
          // If preproject mode is root, then the vertex needs to be transformed relative to
          // some root bone.
          PreprojectMode.ROOT => this.GetWorldMatrix(weights[0].Bone.Root),
          _                   => throw new ArgumentOutOfRangeException()
      };

      return !transformMatrix.IsIdentity ? transformMatrix : null;
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

      double x = localPosition.X;
      double y = localPosition.Y;
      double z = localPosition.Z;
      GlMatrixUtil.ProjectVertex(transformMatrix,
                                 ref x,
                                 ref y,
                                 ref z);
      outPosition.X = (float) x;
      outPosition.Y = (float) y;
      outPosition.Z = (float) z;

      if (outNormal != null && localNormal != null) {
        double nX = localNormal.X;
        double nY = localNormal.Y;
        double nZ = localNormal.Z;
        GlMatrixUtil.ProjectNormal(transformMatrix,
                                   ref nX,
                                   ref nY,
                                   ref nZ);

        outNormal.X = (float) nX;
        outNormal.Y = (float) nY;
        outNormal.Z = (float) nZ;
      }
    }

    public void ProjectVertex(IBone bone,
                              ref double x,
                              ref double y,
                              ref double z) {
      GlMatrixUtil.ProjectVertex(
          this.GetWorldMatrix(bone),
          ref x, ref y, ref z);
    }
  }
}