using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

using fin.data;
using fin.math;
using fin.math.interpolation;


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
        private int frameCount_;

        private readonly IndexableDictionary<IBone, IBoneTracks> boneTracks_ =
            new();

        private readonly Dictionary<IMesh, IMeshTracks> meshTracks_ = new();

        public AnimationImpl() {
          this.BoneTracks = this.boneTracks_;
          this.MeshTracks = this.meshTracks_;
        }

        public string Name { get; set; }

        public int FrameCount {
          get => this.frameCount_;
          set {
            this.frameCount_ = value;
            foreach (var boneTracks in this.boneTracks_) {
              (boneTracks as BoneTracksImpl).FrameCount = value;
            }
          }
        }

        public float FrameRate { get; set; }

        public IReadOnlyIndexableDictionary<IBone, IBoneTracks> BoneTracks {
          get;
        }

        public IBoneTracks AddBoneTracks(IBone bone)
          => AddBoneTracks(bone,
                           AnimationImplConstants.EMPTY_CAPACITY_PER_AXIS,
                           AnimationImplConstants.EMPTY_CAPACITY_PER_AXIS,
                           AnimationImplConstants.EMPTY_CAPACITY_PER_AXIS);

        public IBoneTracks AddBoneTracks(
            IBone bone,
            ReadOnlySpan<int> initialCapacityPerPositionAxis,
            ReadOnlySpan<int> initialCapacityPerRotationAxis,
            ReadOnlySpan<int> initialCapacityPerScaleAxis) {
          var boneTracks = new BoneTracksImpl(
              initialCapacityPerPositionAxis,
              initialCapacityPerRotationAxis,
              initialCapacityPerScaleAxis
              ) {
              FrameCount = this.FrameCount,
          };
          this.boneTracks_[bone] = boneTracks;
          return boneTracks;
        }

        public IReadOnlyDictionary<IMesh, IMeshTracks> MeshTracks { get; }
        public IMeshTracks AddMeshTracks(IMesh mesh)
          => this.meshTracks_[mesh] = new MeshTracksImpl();


        public IReadOnlyDictionary<ITexture, ITextureTracks> TextureTracks 
          => throw new System.NotImplementedException();
        public ITextureTracks AddTextureTracks(ITexture texture) {
          throw new System.NotImplementedException();
        }


        // TODO: Allow setting looping behavior (once, back and forth, etc.)
      }


      public IReadOnlyList<IMorphTarget> MorphTargets { get; }

      public IMorphTarget AddMorphTarget() {
        var morphTarget = new MorphTargetImpl();
        this.morphTargets_.Add(morphTarget);
        return morphTarget;
      }

      private class MorphTargetImpl : IMorphTarget {
        private Dictionary<IVertex, Position> morphs_ = new();

        public MorphTargetImpl() {
          this.Morphs =
              new ReadOnlyDictionary<IVertex, Position>(this.morphs_);
        }

        public string Name { get; set; }
        public IReadOnlyDictionary<IVertex, Position> Morphs { get; }

        public IMorphTarget MoveTo(IVertex vertex, Position position) {
          this.morphs_[vertex] = position;
          return this;
        }
      }
    }

    public class BoneTracksImpl : IBoneTracks {
      public BoneTracksImpl(ReadOnlySpan<int> initialCapacityPerPositionAxis,
                            ReadOnlySpan<int> initialCapacityPerRotationAxis,
                            ReadOnlySpan<int> initialCapacityPerScaleAxis) {
      Positions = new PositionTrack3dImpl(initialCapacityPerPositionAxis);
      Rotations = new RadiansRotationTrack3dImpl(initialCapacityPerRotationAxis);
      Scales = new ScaleTrackImpl(initialCapacityPerScaleAxis);
    }

    public int FrameCount {
        set {
          this.Positions.FrameCount =
              this.Rotations.FrameCount = this.Scales.FrameCount = value;
        }
      }

      public void Set(IBoneTracks other) {
        this.Positions.Set(other.Positions);
        this.Rotations.Set(other.Rotations);
        this.Scales.Set(other.Scales);
      }

      public IPositionTrack3d Positions { get; }
      public IRadiansRotationTrack3d Rotations { get; }
      public IScale3dTrack Scales { get; }

      // TODO: Add pattern for specifying WITH given tracks
    }

    public static class TrackInterpolators {
      public static Position PositionInterpolator(
          Position lhs,
          Position rhs,
          float progress) {
        var fromFrac = 1 - progress;
        var toFrac = progress;

        return new Position(
          lhs.X * fromFrac + rhs.X * toFrac,
          lhs.Y * fromFrac + rhs.Y * toFrac,
          lhs.Z * fromFrac + rhs.Z * toFrac
        );
      }

      // TODO: Implement this.
      public static Quaternion RotationInterpolator(
          IRotation lhs,
          IRotation rhs,
          float progress)
        => QuaternionUtil.CreateZyx(lhs.XRadians, lhs.YRadians, lhs.ZRadians);

      public static Scale ScaleInterpolator(
          Scale lhs,
          Scale rhs,
          float progress) {
        var fromFrac = 1 - progress;
        var toFrac = progress;

        return new Scale (
          lhs.X * fromFrac + rhs.X * toFrac,
          lhs.Y * fromFrac + rhs.Y * toFrac,
          lhs.Z * fromFrac + rhs.Z * toFrac
        );
      }
    }


    public class MeshTracksImpl : IMeshTracks {
      public ITrack<MeshDisplayState> DisplayStates { get; } =
        new TrackImpl<MeshDisplayState>(
            0,
            Interpolator.StairStep<MeshDisplayState>(),
            InterpolatorWithTangents.StairStep<MeshDisplayState>());
    }
  }
}