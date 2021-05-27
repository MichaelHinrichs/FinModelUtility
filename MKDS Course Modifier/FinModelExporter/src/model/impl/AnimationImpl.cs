using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

using fin.math;

namespace fin.model.impl {
  public partial class ModelImpl {
    public IAnimationManager AnimationManager { get; } =
      new AnimationManagerImpl();

    private class AnimationManagerImpl : IAnimationManager {
      private readonly IList<IAnimation> animations_ = new List<IAnimation>();

      public AnimationManagerImpl() {
        this.Animations = new ReadOnlyCollection<IAnimation>(this.animations_);
      }

      public IReadOnlyList<IAnimation> Animations { get; }

      public IAnimation AddAnimation() {
        var animation = new AnimationImpl();
        this.animations_.Add(animation);
        return animation;
      }

      private class AnimationImpl : IAnimation {
        private readonly IDictionary<IBone, IBoneTracks> boneTracks_ =
            new Dictionary<IBone, IBoneTracks>();

        public AnimationImpl() {
          this.BoneTracks =
              new ReadOnlyDictionary<IBone, IBoneTracks>(this.boneTracks_);
        }

        public string Name { get; set; }

        public int FrameCount { get; set; }
        public float Fps { get; set; }

        public IReadOnlyDictionary<IBone, IBoneTracks> BoneTracks { get; }

        public IBoneTracks AddBoneTracks(IBone bone) {
          var boneTracks = new BoneTracksImpl();
          this.boneTracks_[bone] = boneTracks;
          return boneTracks;
        }

        // TODO: Allow setting fps.
        // TODO: Allow setting looping behavior (once, back and forth, etc.)
      }
    }

    private class BoneTracksImpl : IBoneTracks {
      public ITrack<IPosition> Positions { get; } =
        new TrackImpl<IPosition>(BoneTracksImpl.PositionInterpolator_);

      public ITrack<IRotation, Quaternion> Rotations { get; } =
        new TrackImpl<IRotation, Quaternion>(
            BoneTracksImpl.RotationInterpolator_);

      public ITrack<IScale> Scales { get; } =
        new TrackImpl<IScale>(BoneTracksImpl.ScaleInterpolator_);

      private static IPosition PositionInterpolator_(
          IPosition lhs,
          IPosition rhs,
          float progress) {
        var fromFrac = 1 - progress;
        var toFrac = progress;

        return new PositionImpl {
            X = lhs.X * fromFrac + rhs.X * toFrac,
            Y = lhs.Y * fromFrac + rhs.Y * toFrac,
            Z = lhs.Z * fromFrac + rhs.Z * toFrac
        };
      }

      // TODO: Implement this.
      private static Quaternion RotationInterpolator_(
          IRotation lhs,
          IRotation rhs,
          float progress)
        => QuaternionUtil.Create(lhs.XRadians, lhs.YRadians, lhs.ZRadians);

      private static IScale ScaleInterpolator_(
          IScale lhs,
          IScale rhs,
          float progress) {
        var fromFrac = 1 - progress;
        var toFrac = progress;

        return new ScaleImpl {
            X = lhs.X * fromFrac + rhs.X * toFrac,
            Y = lhs.Y * fromFrac + rhs.Y * toFrac,
            Z = lhs.Z * fromFrac + rhs.Z * toFrac
        };
      }

      // TODO: Add pattern for specifying WITH given tracks
    }

    public class TrackImpl<T> : TrackImpl<T, T>, ITrack<T> {
      public TrackImpl(Func<T, T, float, T> interpolator) :
          base(interpolator) {}
    }

    public class TrackImpl<TValue, TInterpolated> :
        ITrack<TValue, TInterpolated> {
      public readonly Func<TValue, TValue, float, TInterpolated> interpolator_;

      private readonly IList<Keyframe<TValue>> keyframesAndValues_ =
          new List<Keyframe<TValue>>();

      public TrackImpl(
          Func<TValue, TValue, float, TInterpolated> interpolator) {
        this.interpolator_ = interpolator;
        this.Keyframes =
            new ReadOnlyCollection<Keyframe<TValue>>(this.keyframesAndValues_);
      }

      public IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

      public void Set(int frame, TValue t) {
        this.FindIndexOfKeyframe_(frame,
                                  out var keyframeIndex,
                                  out _,
                                  out var keyframeDefined,
                                  out var pastEnd);

        var keyframeAndValue = new Keyframe<TValue>(frame, t);
        if (pastEnd) {
          this.keyframesAndValues_.Add(keyframeAndValue);
        } else if (keyframeDefined) {
          this.keyframesAndValues_[keyframeIndex] = keyframeAndValue;
        } else {
          this.keyframesAndValues_.Insert(keyframeIndex, keyframeAndValue);
        }
      }

      public TValue? GetKeyframe(int frame) {
        this.FindIndexOfKeyframe_(frame,
                                  out _,
                                  out var value,
                                  out var keyframeDefined,
                                  out _);

        return keyframeDefined ? value : default;
      }

      public TInterpolated? GetInterpolatedFrame(float frame) {
        this.FindIndexOfKeyframe_((int) frame,
                                  out var fromKeyframeIndex,
                                  out var fromValue,
                                  out var keyframeDefined,
                                  out var pastEnd);

        if (pastEnd) {
          return default;
        }

        if (fromKeyframeIndex == this.keyframesAndValues_.Count - 1) {
          return this.interpolator_(fromValue, fromValue, 0);
        }

        var (toKeyframeIndex, toValue) =
            this.keyframesAndValues_[fromKeyframeIndex + 1];

        return this.interpolator_(fromValue!,
                                  toValue,
                                  (frame - fromKeyframeIndex) /
                                  (toKeyframeIndex - fromKeyframeIndex));
      }

      // TODO: Use a more efficient approach here, e.g. binary search.
      private void FindIndexOfKeyframe_(
          int frame,
          out int keyframeIndex,
          out TValue? value,
          out bool keyframeDefined,
          out bool pastEnd) {
        var keyframeCount = this.keyframesAndValues_.Count;
        for (var i = 0; i < keyframeCount; ++i) {
          var (currentKeyframe, t) = this.keyframesAndValues_[i];

          if (currentKeyframe == frame) {
            keyframeIndex = i;
            value = t;
            keyframeDefined = true;
            pastEnd = false;
            return;
          }

          if (currentKeyframe > frame) {
            keyframeIndex = i;
            value = t;
            keyframeDefined = false;
            pastEnd = false;
            return;
          }
        }

        keyframeIndex = keyframeCount;
        value = default;
        keyframeDefined = false;
        pastEnd = true;
      }
    }
  }
}