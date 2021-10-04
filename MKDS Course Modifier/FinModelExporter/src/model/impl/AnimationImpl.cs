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

    public class BoneTracksImpl : IBoneTracks {
      public IPositionTrack Positions { get; } = new PositionTrackImpl();
      public IRadiansRotationTrack Rotations { get; } = new RadiansRotationTrackImpl();
      public IScaleTrack Scales { get; } = new ScaleTrackImpl();

      // TODO: Add pattern for specifying WITH given tracks
    }

    public static class TrackInterpolators {
      public static float FloatInterpolator(
          float lhs,
          float rhs,
          float progress)
        => lhs * (1 - progress) + rhs * progress;

      public static IPosition PositionInterpolator(
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
      public static Quaternion RotationInterpolator(
          IRotation lhs,
          IRotation rhs,
          float progress)
        => QuaternionUtil.Create(lhs.XRadians, lhs.YRadians, lhs.ZRadians);

      public static IScale ScaleInterpolator(
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
    }
  }
}