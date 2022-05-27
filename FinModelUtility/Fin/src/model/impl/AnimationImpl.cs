using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

using fin.data;
using fin.math;


namespace fin.model.impl {
  public partial class ModelImpl {
    public IAnimationManager AnimationManager { get; } =
      new AnimationManagerImpl();

    private class AnimationManagerImpl : IAnimationManager {
      private readonly IList<IAnimation> animations_ = new List<IAnimation>();

      private readonly IList<IMorphTarget> morphTargets_ =
          new List<IMorphTarget>();

      public AnimationManagerImpl() {
        this.Animations = new ReadOnlyCollection<IAnimation>(this.animations_);
        this.MorphTargets =
            new ReadOnlyCollection<IMorphTarget>(this.morphTargets_);
      }


      public IReadOnlyList<IAnimation> Animations { get; }

      public IAnimation AddAnimation() {
        var animation = new AnimationImpl();
        this.animations_.Add(animation);
        return animation;
      }

      private class AnimationImpl : IAnimation {
        private readonly IndexableDictionary<IBone, IBoneTracks> boneTracks_ =
            new();

        public AnimationImpl() {
          this.BoneTracks = this.boneTracks_;
        }

        public string Name { get; set; }

        public int FrameCount { get; set; }
        public float FrameRate { get; set; }

        public IReadOnlyIndexableDictionary<IBone, IBoneTracks> BoneTracks { get; }

        public IBoneTracks AddBoneTracks(IBone bone) {
          var boneTracks = new BoneTracksImpl();
          this.boneTracks_[bone] = boneTracks;
          return boneTracks;
        }

        // TODO: Allow setting fps.
        // TODO: Allow setting looping behavior (once, back and forth, etc.)
      }


      public IReadOnlyList<IMorphTarget> MorphTargets { get; }

      public IMorphTarget AddMorphTarget() {
        var morphTarget = new MorphTargetImpl();
        this.morphTargets_.Add(morphTarget);
        return morphTarget;
      }

      private class MorphTargetImpl : IMorphTarget {
        private Dictionary<IVertex, IPosition> morphs_ = new();

        public MorphTargetImpl() {
          this.Morphs =
              new ReadOnlyDictionary<IVertex, IPosition>(this.morphs_);
        }

        public string Name { get; set; }
        public IReadOnlyDictionary<IVertex, IPosition> Morphs { get; }

        public IMorphTarget MoveTo(IVertex vertex, IPosition position) {
          this.morphs_[vertex] = position;
          return this;
        }
      }
    }

    public class BoneTracksImpl : IBoneTracks {
      public void Set(IBoneTracks other) {
        this.Positions.Set(other.Positions);
        this.Rotations.Set(other.Rotations);
        this.Scales.Set(other.Scales);
      }

      public IPositionTrack Positions { get; } = new PositionTrackImpl();

      public IRadiansRotationTrack Rotations { get; } =
        new RadiansRotationTrackImpl();

      public IScaleTrack Scales { get; } = new ScaleTrackImpl();

      // TODO: Add pattern for specifying WITH given tracks
    }

    public static class TrackInterpolators {
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