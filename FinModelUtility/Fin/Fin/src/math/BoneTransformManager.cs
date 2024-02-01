﻿using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

using fin.animation;
using fin.data.indexable;
using fin.math.matrix.four;
using fin.math.rotations;
using fin.model;
using fin.ui;

namespace fin.math {
  public interface IBoneTransformManager {
    (IBoneTransformManager, IBone)? Parent { get; }

    void Clear();

    void InitModelVertices(IModel model, bool forcePreproject = false);

    IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IModelAnimation, float)? animationAndFrame,
        AnimationInterpolationConfig? config = null
    );

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone);

    public IReadOnlyFinMatrix4x4? GetTransformMatrix(IReadOnlyVertex vertex);
    public IReadOnlyFinMatrix4x4 GetTransformMatrix(IBoneWeights boneWeights);

    void ProjectVertexPosition(
        IReadOnlyVertex vertex,
        out Position outPosition);

    void ProjectVertexPositionNormal(
        IReadOnlyNormalVertex vertex,
        out Position outPosition,
        out Normal outNormal);

    void ProjectVertexPositionNormalTangent(
        IReadOnlyNormalTangentVertex vertex,
        out Position outPosition,
        out Normal outNormal,
        out Tangent outTangent);


    void ProjectPosition(IBone bone, ref Vector3 xyz);

    void ProjectNormal(IBone bone, ref Vector3 xyz);
  }

  public class BoneTransformManager : IBoneTransformManager {
    // TODO: This is going to be slow, can we put this somewhere else for O(1) access?
    private readonly IndexableDictionary<IBone, IFinMatrix4x4>
        bonesToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToInverseBindMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IFinMatrix4x4>
        boneWeightsToWorldMatrices_ = new();

    private IndexableDictionary<IReadOnlyVertex, IReadOnlyFinMatrix4x4?>
        verticesToWorldMatrices_ = new();

    public (IBoneTransformManager, IBone)? Parent { get; }
    public IReadOnlyFinMatrix4x4 ManagerMatrix { get; }

    public BoneTransformManager() {
      this.ManagerMatrix = FinMatrix4x4.IDENTITY;
    }

    public BoneTransformManager((IBoneTransformManager, IBone) parent) {
      this.Parent = parent;
      var (parentManager, parentBone) = this.Parent.Value;
      this.ManagerMatrix = parentManager.GetWorldMatrix(parentBone);
    }

    public void Clear() {
      this.bonesToWorldMatrices_.Clear();
      this.bonesToInverseBindMatrices_.Clear();
      this.boneWeightsToWorldMatrices_.Clear();
      this.verticesToWorldMatrices_.Clear();
    }

    public void InitModelVertices(IModel model, bool forcePreproject = false) {
      var vertices = model.Skin.Vertices;
      this.verticesToWorldMatrices_ =
          new IndexableDictionary<IReadOnlyVertex, IReadOnlyFinMatrix4x4?>(
              vertices.Count);
      foreach (var vertex in vertices) {
        this.verticesToWorldMatrices_[vertex] =
            DetermineTransformMatrix_(vertex.BoneWeights, forcePreproject);
      }
    }

    private readonly MagFilterInterpolationTrack<Position> positionMagFilterInterpolationTrack_ =
        new(null, Position.Lerp) {
          AnimationInterpolationMagFilter = AnimationInterpolationMagFilter.ORIGINAL_FRAME_RATE_LINEAR
        };

    private readonly MagFilterInterpolationTrack<Quaternion> rotationMagFilterInterpolationTrack_ =
        new(null, Quaternion.Slerp) {
          AnimationInterpolationMagFilter = AnimationInterpolationMagFilter.ORIGINAL_FRAME_RATE_LINEAR
        };

    private readonly MagFilterInterpolationTrack<Scale> scaleMagFilterInterpolationTrack_ =
        new(null, Scale.Lerp) {
          AnimationInterpolationMagFilter = AnimationInterpolationMagFilter.ORIGINAL_FRAME_RATE_LINEAR
        };

    public void CalculateMatrices(IModel model)
      => this.CalculateMatrices(model.Skeleton.Root, model.Skin.BoneWeights, null);

    public IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IModelAnimation, float)? animationAndFrame,
        AnimationInterpolationConfig? config = null
    ) {
      var isFirstPass = animationAndFrame == null;

      var animation = animationAndFrame?.Item1;
      var frame = animationAndFrame?.Item2;

      // TODO: Cache this directly on the bone itself instead.
      var bonesToIndex = new Dictionary<IBone, int>();
      var boneIndex = -1;

      var boneQueue = new Queue<(IBone, Matrix4x4)>();
      boneQueue.Enqueue((rootBone, this.ManagerMatrix.Impl));
      while (boneQueue.Count > 0) {
        var (bone, parentBoneToWorldMatrix) = boneQueue.Dequeue();

        if (!this.bonesToWorldMatrices_.TryGetValue(bone, out var boneToWorldMatrix)) {
          this.bonesToWorldMatrices_[bone] = boneToWorldMatrix = new FinMatrix4x4();
        }

        boneToWorldMatrix.CopyFrom(parentBoneToWorldMatrix);

        Position? animationLocalPosition = null;
        Quaternion? animationLocalRotation = null;
        Scale? animationLocalScale = null;

        // The pose of the animation, if available.
        IBoneTracks? boneTracks = null;
        animation?.BoneTracks.TryGetValue(bone, out boneTracks);
        if (boneTracks != null) {
          // Only gets the values from the animation if the frame is at least partially defined.
          if (boneTracks.Positions?.HasAtLeastOneKeyframe ?? false) {
            this.positionMagFilterInterpolationTrack_.Impl = boneTracks.Positions;
            if (this.positionMagFilterInterpolationTrack_.TryGetInterpolatedFrame(
                    (float) frame,
                    out var outAnimationLocalPosition,
                    config)) {
              animationLocalPosition = outAnimationLocalPosition;
            }
          }

          if (boneTracks.Rotations?.HasAtLeastOneKeyframe ?? false) {
            this.rotationMagFilterInterpolationTrack_.Impl = boneTracks.Rotations;
            if (this.rotationMagFilterInterpolationTrack_.TryGetInterpolatedFrame(
                    (float) frame,
                    out var outAnimationLocalRotation,
                    config)) {
              animationLocalRotation = outAnimationLocalRotation;
            }
          }

          if (boneTracks.Scales?.HasAtLeastOneKeyframe ?? false) {
            this.scaleMagFilterInterpolationTrack_.Impl = boneTracks.Scales;
            if (this.scaleMagFilterInterpolationTrack_.TryGetInterpolatedFrame(
                    (float) frame,
                    out var outAnimationLocalScale,
                    config)) {
              animationLocalScale = outAnimationLocalScale;
            }
          }
        }

        // Uses the animation pose instead of the root pose when available.
        var localPosition = animationLocalPosition ?? bone.LocalPosition;
        var localRotation = animationLocalRotation ??
                            (bone.LocalRotation != null
                                ? QuaternionUtil.Create(bone.LocalRotation)
                                : null);
        var localScale = animationLocalScale ?? bone.LocalScale;

        if (!bone.IgnoreParentScale && !bone.FaceTowardsCamera) {
          var localMatrix = SystemMatrix4x4Util.FromTrs(localPosition,
                                                        localRotation,
                                                        localScale);
          boneToWorldMatrix.MultiplyInPlace(localMatrix);
        } else {
          // Applies translation first, so it's affected by parent rotation/scale.
          var localTranslationMatrix =
              SystemMatrix4x4Util.FromTranslation(localPosition);
          boneToWorldMatrix.MultiplyInPlace(localTranslationMatrix);

          // Extracts translation/rotation/scale.
          boneToWorldMatrix.CopyTranslationInto(out var translationBuffer);
          Quaternion rotationBuffer;
          if (bone.FaceTowardsCamera) {
            var camera = Camera.Instance;
            var yawDegrees = camera?.YawDegrees ?? 0;
            var yawRadians = yawDegrees * FinTrig.DEG_2_RAD;
            var yawRotation =
                Quaternion.CreateFromYawPitchRoll(yawRadians, 0, 0);

            rotationBuffer = yawRotation * bone.FaceTowardsCameraAdjustment;
          } else {
            boneToWorldMatrix.CopyRotationInto(out rotationBuffer);
          }

          Scale scaleBuffer;
          if (bone.IgnoreParentScale) {
            scaleBuffer = new Scale(1);
          } else {
            boneToWorldMatrix.CopyScaleInto(out scaleBuffer);
          }

          // Gets final matrix.
          FinMatrix4x4Util.FromTrs(
              translationBuffer,
              rotationBuffer,
              scaleBuffer,
              boneToWorldMatrix);
          boneToWorldMatrix.MultiplyInPlace(
              SystemMatrix4x4Util.FromTrs(null,
                                       localRotation,
                                       localScale));
        }

        if (isFirstPass) {
          this.bonesToInverseBindMatrices_[bone] = boneToWorldMatrix.CloneAndInvert();
        }

        bonesToIndex[bone] = boneIndex++;

        foreach (var child in bone.Children) {
          // TODO: Use a pool of matrices to prevent unneeded instantiations.
          boneQueue.Enqueue((child, boneToWorldMatrix.Impl));
        }
      }

      foreach (var boneWeights in boneWeightsList) {
        if (!this.boneWeightsToWorldMatrices_.TryGetValue(
                boneWeights,
                out var boneWeightMatrix)) {
          this.boneWeightsToWorldMatrices_[boneWeights] =
              boneWeightMatrix = new FinMatrix4x4();
        }

        boneWeightMatrix.SetZero();

        var weights = boneWeights.Weights;
        if (weights.Count == 1) {
          var weight = weights[0];
          var bone = weight.Bone;

          var skinToBoneMatrix = weight.InverseBindMatrix ??
                                 this.bonesToInverseBindMatrices_[bone];
          var boneMatrix = this.GetWorldMatrix(bone);

          boneMatrix.MultiplyIntoBuffer(skinToBoneMatrix, boneWeightMatrix);
        } else {
          foreach (var weight in weights) {
            var bone = weight.Bone;

            var skinToBoneMatrix = weight.InverseBindMatrix ??
                                   this.bonesToInverseBindMatrices_[bone];
            var boneMatrix = this.GetWorldMatrix(bone);

            var skinToWorldMatrix = (boneMatrix.Impl * skinToBoneMatrix.Impl) * weight.Weight;
            boneWeightMatrix.AddInPlace(skinToWorldMatrix);
          }
        }
      }

      return bonesToIndex;
    }

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    public IReadOnlyFinMatrix4x4? GetTransformMatrix(IReadOnlyVertex vertex)
      => this.verticesToWorldMatrices_[vertex];

    public IReadOnlyFinMatrix4x4 GetTransformMatrix(IBoneWeights boneWeights)
      => this.boneWeightsToWorldMatrices_[boneWeights];

    private IReadOnlyFinMatrix4x4? DetermineTransformMatrix_(
        IBoneWeights? boneWeights,
        bool forcePreproject = false) {
      var weights = boneWeights?.Weights;
      var preproject =
          (boneWeights?.VertexSpace != VertexSpace.RELATIVE_TO_WORLD ||
           forcePreproject) &&
          weights?.Count > 0;

      if (!preproject) {
        return null;
      }

      return this.boneWeightsToWorldMatrices_[boneWeights!];
    }

    public void ProjectVertexPosition(
        IReadOnlyVertex vertex,
        out Position outPosition) {
      outPosition = vertex.LocalPosition;

      var finTransformMatrix = this.GetTransformMatrix(vertex);
      if (finTransformMatrix == null) {
        return;
      }

      var transformMatrix = finTransformMatrix.Impl;
      ProjectionUtil.ProjectPosition(transformMatrix, ref outPosition);
    }

    public void ProjectVertexPositionNormal(
        IReadOnlyNormalVertex vertex,
        out Position outPosition,
        out Normal outNormal) {
      outPosition = vertex.LocalPosition;
      outNormal = vertex.LocalNormal.GetValueOrDefault();

      var finTransformMatrix = this.GetTransformMatrix(vertex);
      if (finTransformMatrix == null) {
        return;
      }

      var transformMatrix = finTransformMatrix.Impl;
      ProjectionUtil.ProjectPosition(transformMatrix, ref outPosition);
      if (vertex.LocalNormal.HasValue) {
        ProjectionUtil.ProjectNormal(transformMatrix, ref outNormal);
      }
    }

    public void ProjectVertexPositionNormalTangent(
        IReadOnlyNormalTangentVertex vertex,
        out Position outPosition,
        out Normal outNormal,
        out Tangent outTangent) {
      outPosition = vertex.LocalPosition;

      outNormal = vertex.LocalNormal.GetValueOrDefault();
      outTangent = vertex.LocalTangent.GetValueOrDefault();

      var finTransformMatrix = this.GetTransformMatrix(vertex);
      if (finTransformMatrix == null) {
        return;
      }

      var transformMatrix = finTransformMatrix.Impl;
      ProjectionUtil.ProjectPosition(transformMatrix, ref outPosition);
      if (vertex.LocalNormal.HasValue) {
        ProjectionUtil.ProjectNormal(transformMatrix, ref outNormal);
      }

      if (vertex.LocalTangent.HasValue) {
        ProjectionUtil.ProjectTangent(transformMatrix, ref outTangent);
      }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectPosition(IBone bone, ref Position xyz)
      => ProjectionUtil.ProjectPosition(this.GetWorldMatrix(bone).Impl, ref xyz);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectPosition(IBone bone, ref Vector3 xyz)
      => ProjectionUtil.ProjectPosition(this.GetWorldMatrix(bone).Impl, ref xyz);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectPosition(IBoneWeights bone, ref Vector3 xyz)
      => ProjectionUtil.ProjectPosition(this.GetTransformMatrix(bone).Impl, ref xyz);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectNormal(IBone bone, ref Normal xyz)
      => ProjectionUtil.ProjectNormal(this.GetWorldMatrix(bone).Impl, ref xyz);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectNormal(IBone bone, ref Vector3 xyz)
      => ProjectionUtil.ProjectNormal(this.GetWorldMatrix(bone).Impl, ref xyz);
  }
}