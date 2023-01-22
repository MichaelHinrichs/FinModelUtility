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

        public IBoneTracks AddBoneTracks(IBone bone) {
          var boneTracks = new BoneTracksImpl {
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

      public IPositionTrack3d Positions { get; } = new PositionTrack3dImpl();

      public IRadiansRotationTrack3d Rotations { get; } =
        new RadiansRotationTrack3dImpl();

      public IScale3dTrack Scales { get; } = new ScaleTrackImpl();

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


    public class MeshTracksImpl : IMeshTracks {
      public ITrack<MeshDisplayState> DisplayStates { get; } =
        new TrackImpl<MeshDisplayState>(
          Interpolator.StairStep<MeshDisplayState>(), 
          InterpolatorWithTangents.StairStep<MeshDisplayState>());
    }
  }
}