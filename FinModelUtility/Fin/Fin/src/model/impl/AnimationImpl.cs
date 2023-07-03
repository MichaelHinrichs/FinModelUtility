using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using fin.data;
using fin.math.interpolation;


namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
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
          => this.boneTracks_[bone] =
              new BoneTracksImpl(bone) { FrameCount = this.FrameCount };

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
      private readonly IBone bone_;
      private int frameCount_;

      public BoneTracksImpl(IBone bone) {
        this.bone_ = bone;
      }

      public int FrameCount {
        set {
          this.frameCount_ = value;

          if (this.Positions != null) {
            this.Positions.FrameCount = value;
          }

          if (this.Rotations != null) {
            this.Rotations.FrameCount = value;
          }

          if (this.Scales != null) {
            this.Scales.FrameCount = value;
          }
        }
      }

      public IPositionTrack3d? Positions { get; private set; }
      public IRotationTrack3d? Rotations { get; private set; }
      public IScale3dTrack? Scales { get; private set; }

      public ICombinedPositionAxesTrack3d UseCombinedPositionAxesTrack(int initialCapacity)
        => (ICombinedPositionAxesTrack3d) (this.Positions =
            new CombinedPositionAxesTrack3dImpl(initialCapacity) {
                FrameCount = this.frameCount_
            });

      public ISeparatePositionAxesTrack3d UseSeparatePositionAxesTrack(int initialCapacity)
        => this.UseSeparatePositionAxesTrack(initialCapacity,
                                  initialCapacity,
                                  initialCapacity);

      public ISeparatePositionAxesTrack3d UseSeparatePositionAxesTrack(
          int initialXCapacity,
          int initialYCapacity,
          int initialZCapacity) {
        Span<int> initialAxisCapacities = stackalloc int[3];
        initialAxisCapacities[0] = initialXCapacity;
        initialAxisCapacities[1] = initialYCapacity;
        initialAxisCapacities[2] = initialZCapacity;

        return (ISeparatePositionAxesTrack3d) (this.Positions =
            new SeparatePositionAxesTrack3dImpl(this.bone_, initialAxisCapacities) {
                FrameCount = this.frameCount_
            });
      }


      public IQuaternionRotationTrack3d UseQuaternionRotationTrack(
          int initialCapacity)
        => (IQuaternionRotationTrack3d) (this.Rotations =
            new QuaternionRotationTrack3dImpl(initialCapacity) {
                FrameCount = this.frameCount_
            });

      public IQuaternionAxesRotationTrack3d UseQuaternionAxesRotationTrack()
        => (IQuaternionAxesRotationTrack3d) (this.Rotations =
            new QuaternionAxesRotationTrack3dImpl(this.bone_) {
                FrameCount = this.frameCount_
            });


      public IEulerRadiansRotationTrack3d UseEulerRadiansRotationTrack(
          int initialCapacity)
        => this.UseEulerRadiansRotationTrack(initialCapacity,
                                             initialCapacity,
                                             initialCapacity);

      public IEulerRadiansRotationTrack3d UseEulerRadiansRotationTrack(
          int initialXCapacity,
          int initialYCapacity,
          int initialZCapacity) {
        Span<int> initialAxisCapacities = stackalloc int[3];
        initialAxisCapacities[0] = initialXCapacity;
        initialAxisCapacities[1] = initialYCapacity;
        initialAxisCapacities[2] = initialZCapacity;

        return (IEulerRadiansRotationTrack3d) (this.Rotations =
            new EulerRadiansRotationTrack3dImpl(
                this.bone_,
                initialAxisCapacities) {
                FrameCount = this.frameCount_
            });
      }


      public IScale3dTrack UseScaleTrack(
          int initialCapacity)
        => this.UseScaleTrack(initialCapacity,
                              initialCapacity,
                              initialCapacity);

      public IScale3dTrack UseScaleTrack(
          int initialXCapacity,
          int initialYCapacity,
          int initialZCapacity) {
        Span<int> initialAxisCapacities = stackalloc int[3];
        initialAxisCapacities[0] = initialXCapacity;
        initialAxisCapacities[1] = initialYCapacity;
        initialAxisCapacities[2] = initialZCapacity;

        return this.Scales = new ScaleTrackImpl(
            this.bone_,
            initialAxisCapacities) {
            FrameCount = this.frameCount_
        };
      }


      // TODO: Add pattern for specifying WITH given tracks
    }

    public class MeshTracksImpl : IMeshTracks {
      public IInputOutputTrack<MeshDisplayState,
          StairStepInterpolator<MeshDisplayState>> DisplayStates { get; } =
        new InputOutputTrackImpl<MeshDisplayState,
            StairStepInterpolator<MeshDisplayState>>(
            0,
            new StairStepInterpolator<MeshDisplayState>());
    }
  }
}