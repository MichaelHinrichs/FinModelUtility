using System;
using System.Collections.Generic;
using fin.data;


namespace fin.model {
  public interface IAnimationManager {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();

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

    IReadOnlyIndexableDictionary<IBone, IBoneTracks> BoneTracks { get; }
    IBoneTracks AddBoneTracks(IBone bone);

    IBoneTracks AddBoneTracks(IBone bone,
                              ReadOnlySpan<int> initialCapacityPerPositionAxis,
                              ReadOnlySpan<int> initialCapacityPerRotationAxis,
                              ReadOnlySpan<int> initialCapacityPerScaleAxis);

    IReadOnlyDictionary<IMesh, IMeshTracks> MeshTracks { get; }
    IMeshTracks AddMeshTracks(IMesh mesh);

    IReadOnlyDictionary<ITexture, ITextureTracks> TextureTracks { get; }
    ITextureTracks AddTextureTracks(ITexture texture);

    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }

  public interface IBoneTracks {
    void Set(IBoneTracks other);

    // TODO: Should these be null if empty?
    IPositionTrack3d Positions { get; }
    IEulerRadiansRotationTrack3d Rotations { get; }
    IScale3dTrack Scales { get; }
  }


  public enum MeshDisplayState {
    UNDEFINED,
    HIDDEN,
    VISIBLE,
  }

  public interface IMeshTracks {
    ITrack<MeshDisplayState> DisplayStates { get; }
  }



  public interface ITextureTracks {
  }


  // TODO: Add a track for animating params, e.g. textures, UVs, frames.
}