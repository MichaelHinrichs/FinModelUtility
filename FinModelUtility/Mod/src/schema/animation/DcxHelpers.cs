using fin.model;
using fin.util.optional;


namespace mod.schema.animation {
  public static class DcxHelpers {
    public static IAnimation AddAnimation(
        IBone[] bones,
        IAnimationManager animationManager,
        IDcx dcx) {
      var isDck = dcx is Dck;
      var dcxAnimationData = dcx.AnimationData;

      var animation = animationManager.AddAnimation();
      animation.Name = dcx.Name;
      animation.FrameCount = (int)dcxAnimationData.FrameCount;
      animation.FrameRate = 30;

      foreach (var jointData in dcxAnimationData.JointDataList) {
        var jointIndex = jointData.JointIndex;

        var jointKeyframes = animation.AddBoneTracks(bones[jointIndex]);

        Keyframe<float>[][] frames;

        frames = DcxHelpers.ReadKeyframes_(
            isDck,
            dcxAnimationData,
            jointData.ScaleAxes,
            dcxAnimationData.ScaleValues);
        DcxHelpers.MergeKeyframesToScaleTrack(
            frames,
            jointKeyframes.Scales);

        frames = DcxHelpers.ReadKeyframes_(
            isDck,
            dcxAnimationData,
            jointData.RotationAxes,
            dcxAnimationData.RotationValues);
        DcxHelpers.MergeKeyframesToRotationTrack(
            frames,
            jointKeyframes.Rotations);

        frames = DcxHelpers.ReadKeyframes_(
            isDck,
            dcxAnimationData,
            jointData.PositionAxes,
            dcxAnimationData.PositionValues);
        DcxHelpers.MergeKeyframesToPositionTrack(
            frames,
            jointKeyframes.Positions);

        animation.AddBoneTracks(bones[jointIndex]).Set(jointKeyframes);
      }

      return animation;
    }

    private static Keyframe<float>[][] ReadKeyframes_(
        bool isDck,
        IDcxAnimationData animationData,
        IDcxAxes axes, 
        float[] values) {
      var frames = new Keyframe<float>[3][];
      for (var i = 0; i < 3; ++i) {
        var axis = axes.Axes[i];

        var frameCount = axis.FrameCount;
        var frameOffset = axis.FrameOffset;

        var sparse = isDck && frameCount != 1;
        frames[i] = !sparse
                        ? DcxHelpers.ReadDenseFrames(
                            values,
                            frameOffset,
                            frameCount)
                        : DcxHelpers.ReadSparseFrames(
                            values,
                            frameOffset,
                            frameCount);
      }
      return frames;
    }

    public static Keyframe<float>[] ReadDenseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe<float>[count];
      for (var i = 0; i < count; ++i) {
        keyframes[i] =
            new Keyframe<float>(i,
                                values[offset + i],
                                Optional.None<float>(),
                                Optional.None<float>());
      }
      return keyframes;
    }

    public static Keyframe<float>[] ReadSparseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe<float>[count];
      for (var i = 0; i < count; ++i) {
        var index = (int) values[offset + 3 * i];
        var value = values[offset + 3 * i + 1];

        // TODO: This is a guess, is this actually right?
        // The tangents are HUGE, have to be scaled down by the FPS.
        var tangent = values[offset + 3 * i + 2] / 30f;

        var optionalTangent = Optional.Of(tangent);
        keyframes[i] =
            new Keyframe<float>(index, value, optionalTangent, optionalTangent);
      }
      return keyframes;
    }

    // TODO: Do this sparsely
    public static void MergeKeyframesToPositionTrack(
        Keyframe<float>[][] positionKeyframes,
        IPositionTrack positionTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in positionKeyframes[i]) {
          positionTrack.Set(keyframe.Frame,
                            i,
                            keyframe.Value,
                            keyframe.IncomingTangent,
                            keyframe.OutgoingTangent);
        }
      }
    }

    public static void MergeKeyframesToRotationTrack(
        Keyframe<float>[][] rotationKeyframes,
        IRadiansRotationTrack rotationTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in rotationKeyframes[i]) {
          rotationTrack.Set(keyframe.Frame,
                            i,
                            keyframe.Value,
                            keyframe.IncomingTangent,
                            keyframe.OutgoingTangent);
        }
      }
    }

    public static void MergeKeyframesToScaleTrack(
        Keyframe<float>[][] scaleKeyframes,
        IScaleTrack scaleTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in scaleKeyframes[i]) {
          scaleTrack.Set(keyframe.Frame,
                         i,
                         keyframe.Value,
                         keyframe.IncomingTangent,
                         keyframe.OutgoingTangent);
        }
      }
    }
  }
}