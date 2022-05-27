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

    private readonly IndexableDictionary<IBone, IReadOnlyFinMatrix4x4>
        bonesToInverseNeutralWorldMatrices_ = new();

    public IDictionary<IBone, int> CalculateMatrices(
        IBone rootBone,
        (IAnimation, float)? animationAndFrame,
        bool addAnimationToRootPose = false) {
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
            boneTracks?.Positions.GetInterpolatedFrame((float) frame);
        var animationLocalRotation =
            boneTracks?.Rotations.GetInterpolatedFrame((float) frame);
        var animationLocalScale =
            boneTracks?.Scales.GetInterpolatedFrame((float) frame);

        IFinMatrix4x4 localMatrix;
        // Uses the animation pose instead of the root pose when available.
        if (!addAnimationToRootPose) {
          var localPosition = animationLocalPosition ?? boneLocalPosition;
          var localRotation = animationLocalRotation ?? boneLocalRotation;
          var localScale = animationLocalScale ?? boneLocalScale;

          localMatrix =
              MatrixTransformUtil.FromTrs(localPosition,
                                          localRotation,
                                          localScale);
        } 
        // Adds the animation pose on top of the root pose.
        else {
          // TODO: This isn't currently working...
          localMatrix = MatrixTransformUtil.FromTrs(
              new[] {boneLocalPosition, animationLocalPosition},
              new[] {boneLocalRotation, animationLocalRotation},
              new[] {boneLocalScale, animationLocalScale});
        }

        matrix.MultiplyInPlace(localMatrix);

        this.bonesToLocalMatrices_[bone] = localMatrix;
        this.bonesToWorldMatrices_[bone] = matrix;
        if (animation == null) {
          this.bonesToInverseNeutralWorldMatrices_[bone] =
              matrix.CloneAndInvert();
        }
        bonesToIndex[bone] = boneIndex++;

        foreach (var child in bone.Children) {
          // TODO: Use a pool of matrices to prevent unneeded instantiations.
          boneQueue.Enqueue((child, matrix.Clone()));
        }
      }

      return bonesToIndex;
    }

    public IReadOnlyFinMatrix4x4 GetLocalMatrix(IBone bone)
      => this.bonesToLocalMatrices_[bone];

    public IReadOnlyFinMatrix4x4 GetWorldMatrix(IBone bone)
      => this.bonesToWorldMatrices_[bone];

    private IFinMatrix4x4 buffer_ = new FinMatrix4x4();

    public void ProjectVertex(
        IVertex vertex,
        IPosition outPosition,
        INormal? outNormal = null,
        bool forcePreproject = false) {
      var weights = vertex.Weights;
      var preproject =
          (vertex.PreprojectMode != PreprojectMode.NONE || forcePreproject) &&
          weights?.Count > 0;

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

      IReadOnlyFinMatrix4x4 transformMatrix;
      switch (vertex.PreprojectMode) {
        // If preproject mode is none, then the vertices are already in the same position as the bones.
        // To calculate the animation, we have to first "undo" the root pose via an inverted matrix. 
        case PreprojectMode.NONE: {
          // TODO: Precompute these in a shared way somehow.
          var mergedMatrix = new FinMatrix4x4();
          var mergedNeutralMatrix = new FinMatrix4x4();
          foreach (var weight in weights) {
            var boneMatrix = this.GetWorldMatrix(weight.Bone);

            var skinToWorldMatrix = buffer_;
            boneMatrix.CopyInto(this.buffer_);
            buffer_.MultiplyInPlace(weight.Weight);
            mergedMatrix.AddInPlace(skinToWorldMatrix);

            var inverseNeutralMatrix =
                this.bonesToInverseNeutralWorldMatrices_[weight.Bone];
            var inverseSkinToNeutralWorldMatrix = buffer_;
            inverseNeutralMatrix.CopyInto(this.buffer_);
            buffer_.MultiplyInPlace(weight.Weight);
            mergedNeutralMatrix.AddInPlace(
                inverseSkinToNeutralWorldMatrix);
          }

          transformMatrix = mergedMatrix.MultiplyInPlace(mergedNeutralMatrix);

          break;
        }
        // If preproject mode is bone, then we need to transform the vertex by one or more bones.
        case PreprojectMode.BONE: {
          if (weights.Count == 1) {
            transformMatrix = this.GetWorldMatrix(weights[0].Bone);
          } else {
            // TODO: Precompute these in a shared way somehow.
            var mergedMatrix = new FinMatrix4x4();
            foreach (var weight in weights) {
              var skinToBoneMatrix = weight.SkinToBone;
              var boneMatrix = this.GetWorldMatrix(weight.Bone);

              var skinToWorldMatrix = boneMatrix
                                      .CloneAndMultiply(skinToBoneMatrix)
                                      .MultiplyInPlace(weight.Weight);

              mergedMatrix.AddInPlace(skinToWorldMatrix);
            }
            transformMatrix = mergedMatrix;
          }
          break;
        }
        // If preproject mode is root, then the vertex needs to be transformed relative to
        // some root bone.
        case PreprojectMode.ROOT: {
          transformMatrix = this.GetWorldMatrix(weights[0].Bone.Root);
          break;
        }
        default: throw new ArgumentOutOfRangeException();
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