using System.Collections.Generic;

using fin.data;
using fin.math.interpolation;

namespace fin.model {
  public interface IAnimationManager {
    IReadOnlyList<IModelAnimation> Animations { get; }
    IModelAnimation AddAnimation();

    IReadOnlyList<IMorphTarget> MorphTargets { get; }
    IMorphTarget AddMorphTarget();
  }

  public interface IMorphTarget {
    string Name { get; set; }

    IReadOnlyDictionary<IVertex, Position> Morphs { get; }
    IMorphTarget MoveTo(IVertex vertex, Position position);
  }

  public interface IAnimation {
    string Name { get; set; }

    int FrameCount { get; set; }
    float FrameRate { get; set; }
  }

  public interface IModelAnimation : IAnimation {
    IReadOnlyIndexableDictionary<IBone, IBoneTracks> BoneTracks { get; }
    IBoneTracks AddBoneTracks(IBone bone);

    IReadOnlyDictionary<IMesh, IMeshTracks> MeshTracks { get; }
    IMeshTracks AddMeshTracks(IMesh mesh);

    IReadOnlyDictionary<ITexture, ITextureTracks> TextureTracks { get; }
    ITextureTracks AddTextureTracks(ITexture texture);

    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }



  public interface IAnimationData {
    IAnimation Animation { get; }
  }

  public interface IBoneTracks : IAnimationData {
    IBone Bone { get; }

    IPositionTrack3d? Positions { get; }
    IRotationTrack3d? Rotations { get; }
    IScale3dTrack? Scales { get; }

    ICombinedPositionAxesTrack3d UseCombinedPositionAxesTrack(int initialCapacity = 0);
    ISeparatePositionAxesTrack3d UseSeparatePositionAxesTrack(int initialCapacity = 0);
    ISeparatePositionAxesTrack3d UseSeparatePositionAxesTrack(
        int initialXCapacity,
        int initialYCapacity,
        int initialZCapacity);

    IQuaternionRotationTrack3d UseQuaternionRotationTrack(int initialCapacity = 0);

    IQuaternionAxesRotationTrack3d UseQuaternionAxesRotationTrack();

    IEulerRadiansRotationTrack3d UseEulerRadiansRotationTrack(int initialCapacity = 0);

    IEulerRadiansRotationTrack3d UseEulerRadiansRotationTrack(
        int initialXCapacity,
        int initialYCapacity,
        int initialZCapacity);

    IScale3dTrack UseScaleTrack(int initialCapacity = 0);
    IScale3dTrack UseScaleTrack(
        int initialXCapacity,
        int initialYCapacity,
        int initialZCapacity);
  }


  public enum MeshDisplayState {
    UNDEFINED,
    HIDDEN,
    VISIBLE,
  }

  public interface IMeshTracks : IAnimationData {
    IInputOutputTrack<MeshDisplayState, StairStepInterpolator<MeshDisplayState>>
        DisplayStates { get; }
  }



  public interface ITextureTracks : IAnimationData {
  }


  // TODO: Add a track for animating params, e.g. textures, UVs, frames.
}