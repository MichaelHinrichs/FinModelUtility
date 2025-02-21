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
  public enum BoneWeightTransformType {
    FOR_EXPORT_OR_CPU_PROJECTION,
    FOR_RENDERING,
  }

  public interface IVertexProjector {
    void ProjectVertexPosition(
        IReadOnlyVertex vertex,
        out Position outPosition);

    void ProjectVertexPositionNormal(
        IReadOnlyNormalVertex vertex,
        out Position outPosition,
        out Normal outNormal);

    void ProjectVertexPositionNormalTangent(
        IVertexAccessor vertex,
        out Position outPosition,
        out Normal outNormal,
        out Tangent outTangent);


    void ProjectPosition(IBone bone, ref Vector3 xyz);

    void ProjectNormal(IBone bone, ref Vector3 xyz);
  }

  public interface IReadOnlyBoneTransformManager : IVertexProjector {
    (IReadOnlyBoneTransformManager, IBone)? Parent { get; }

    IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone);

    IReadOnlyFinMatrix4x4 GetLocalToWorldMatrix(IBone bone);
    IReadOnlyFinMatrix4x4 GetInverseBindMatrix(IBone bone);
  }

  public interface IBoneTransformManager : IReadOnlyBoneTransformManager {
    void Clear();

    // TODO: Switch this to a thing that returns a projector instance
    void CalculateStaticMatricesForManualProjection(
        IModel model,
        bool forcePreproject = false);
    void CalculateStaticMatricesForRendering(IModel model);

    void CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IModelAnimation, float)? animationAndFrame,
        BoneWeightTransformType boneWeightTransformType,
        AnimationInterpolationConfig? config = null
    );
  }

  public class BoneTransformManager : IBoneTransformManager {
    // TODO: This is going to be slow, can we put this somewhere else for O(1) access?
    private readonly IndexableDictionary<IBone, IFinMatrix4x4>
        bonesToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToInverseWorldMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IFinMatrix4x4>
        boneWeightsToWorldMatrices_ = new();

    private readonly IndexableDictionary<IBoneWeights, IFinMatrix4x4>
        boneWeightsInverseMatrices_ = new();

    private IndexableDictionary<IReadOnlyVertex, IReadOnlyFinMatrix4x4?>
        verticesToWorldMatrices_ = new();

    public (IReadOnlyBoneTransformManager, IBone)? Parent { get; }
    public IReadOnlyFinMatrix4x4 ManagerMatrix { get; }

    public BoneTransformManager() {
      this.ManagerMatrix = FinMatrix4x4.IDENTITY;
    }

    public BoneTransformManager((IReadOnlyBoneTransformManager, IBone) parent) {
      this.Parent = parent;
      var (parentManager, parentBone) = this.Parent.Value;
      this.ManagerMatrix = parentManager.GetWorldMatrix(parentBone);
    }

    public void Clear() {
      this.bonesToWorldMatrices_.Clear();
      this.bonesToInverseWorldMatrices_.Clear();
      this.boneWeightsToWorldMatrices_.Clear();
      this.boneWeightsInverseMatrices_.Clear();
      this.verticesToWorldMatrices_.Clear();
    }

    private void InitModelVertices_(IModel model, bool forcePreproject = false) {
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

    public void CalculateStaticMatricesForManualProjection(
        IModel model,
        bool forcePreproject = false) {
      this.CalculateMatrices(
          model.Skeleton.Root,
          model.Skin.BoneWeights,
          null,
          BoneWeightTransformType.FOR_EXPORT_OR_CPU_PROJECTION);
      this.InitModelVertices_(model, forcePreproject);
    }

    public void CalculateStaticMatricesForRendering(IModel model) {
      this.CalculateMatrices(
          model.Skeleton.Root,
          model.Skin.BoneWeights,
          null,
          BoneWeightTransformType.FOR_RENDERING);
      this.InitModelVertices_(model);
    }

    public void CalculateMatrices(
        IBone rootBone,
        IReadOnlyList<IBoneWeights> boneWeightsList,
        (IModelAnimation, float)? animationAndFrame,
        BoneWeightTransformType boneWeightTransformType,
        AnimationInterpolationConfig? config = null
    ) {
      var isFirstPass = animationAndFrame == null;

      var animation = animationAndFrame?.Item1;
      var frame = animationAndFrame?.Item2;

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

        if (bone is { IgnoreParentScale: false, FaceTowardsCamera: false }) {
          var localMatrix = SystemMatrix4x4Util.FromTrs(localPosition,
                                                        localRotation,
                                                        localScale);
          boneToWorldMatrix.MultiplyInPlace(localMatrix);
        } else {
          boneToWorldMatrix.ApplyTrsWithFancyBoneEffects(bone,
                                                         localPosition,
                                                         localRotation,
                                                         localScale);
        }

        if (isFirstPass) {
          this.bonesToInverseWorldMatrices_[bone] = boneToWorldMatrix.CloneAndInvert();
        }

        foreach (var child in bone.Children) {
          boneQueue.Enqueue((child, boneToWorldMatrix.Impl));
        }
      }

      if (isFirstPass) {
        foreach (var boneWeights in boneWeightsList) {
          if (!this.boneWeightsInverseMatrices_.TryGetValue(
                  boneWeights,
                  out var boneWeightInverseMatrix)) {
            this.boneWeightsInverseMatrices_[boneWeights] =
                boneWeightInverseMatrix = new FinMatrix4x4();
          }

          boneWeightInverseMatrix.SetZero();
          foreach (var boneWeight in boneWeights.Weights) {
            var bone = boneWeight.Bone;
            var weight = boneWeight.Weight;

            var inverseMatrix = boneWeight.InverseBindMatrix ?? this.bonesToInverseWorldMatrices_[bone];
            boneWeightInverseMatrix.AddInPlace(inverseMatrix.Impl * weight);
          }
        }
      }

      if (boneWeightTransformType == BoneWeightTransformType.FOR_EXPORT_OR_CPU_PROJECTION || isFirstPass) {
        foreach (var boneWeights in boneWeightsList) {
          if (!this.boneWeightsToWorldMatrices_.TryGetValue(
                  boneWeights,
                  out var boneWeightMatrix)) {
            this.boneWeightsToWorldMatrices_[boneWeights] =
                boneWeightMatrix = new FinMatrix4x4();
          }

          boneWeightMatrix.SetZero();
          foreach (var boneWeight in boneWeights.Weights) {
            var bone = boneWeight.Bone;
            var weight = boneWeight.Weight;

            var inverseMatrix = boneWeight.InverseBindMatrix ?? this.bonesToInverseWorldMatrices_[bone];
            boneWeightMatrix.AddInPlace(
            (inverseMatrix.Impl * this.bonesToWorldMatrices_[bone].Impl) * weight);
          }
        }
      }
    }

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    public IReadOnlyFinMatrix4x4? GetTransformMatrix(IReadOnlyVertex vertex)
      => this.verticesToWorldMatrices_[vertex];

    public IReadOnlyFinMatrix4x4 GetTransformMatrix(IBoneWeights boneWeights)
      => this.boneWeightsToWorldMatrices_[boneWeights];

    public IReadOnlyFinMatrix4x4 GetInverseBindMatrix(IBone bone)
      => this.bonesToInverseWorldMatrices_[bone];

    public IReadOnlyFinMatrix4x4 GetLocalToWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

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
        IVertexAccessor vertex,
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
    public void ProjectNormal(IBone bone, ref Normal xyz)
      => ProjectionUtil.ProjectNormal(this.GetWorldMatrix(bone).Impl, ref xyz);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProjectNormal(IBone bone, ref Vector3 xyz)
      => ProjectionUtil.ProjectNormal(this.GetWorldMatrix(bone).Impl, ref xyz);
  }

  public static class BoneTransformManagerExtensions {
    public static void ApplyTrsWithFancyBoneEffects(
        this IFinMatrix4x4? matrix,
        IBone bone,
        in Position localPosition,
        in Quaternion? localRotation,
        in Scale? localScale) {
      if (matrix == null) {
        return;
      }

      // Applies translation first, so it's affected by parent rotation/scale.
      var localTranslationMatrix =
          SystemMatrix4x4Util.FromTranslation(localPosition);
      matrix.MultiplyInPlace(localTranslationMatrix);

      // Extracts translation/rotation/scale.
      matrix.CopyTranslationInto(out var translationBuffer);
      Quaternion rotationBuffer;
      if (bone.FaceTowardsCamera) {
        var camera = Camera.Instance;
        var yawDegrees = camera?.YawDegrees ?? 0;
        var yawRadians = yawDegrees * FinTrig.DEG_2_RAD;
        var yawRotation =
            Quaternion.CreateFromYawPitchRoll(yawRadians, 0, 0);

        rotationBuffer = yawRotation * bone.FaceTowardsCameraAdjustment;
      } else {
        matrix.CopyRotationInto(out rotationBuffer);
      }

      Scale scaleBuffer;
      if (bone.IgnoreParentScale) {
        scaleBuffer = new Scale(1);
      } else {
        matrix.CopyScaleInto(out scaleBuffer);
      }

      // Gets final matrix.
      FinMatrix4x4Util.FromTrs(
          translationBuffer,
          rotationBuffer,
          scaleBuffer,
          matrix);
      matrix.MultiplyInPlace(
          SystemMatrix4x4Util.FromTrs(null,
                                            localRotation,
                                            localScale));
    }
  }
}