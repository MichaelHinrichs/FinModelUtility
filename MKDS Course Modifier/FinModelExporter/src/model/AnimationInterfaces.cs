using System.Collections.Generic;
using System.Numerics;

namespace fin.model {
  public interface IAnimationManager {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();
  }

  public interface IAnimation {
    string Name { get; set; }
    
    int FrameCount { get; set; }
    float Fps { get; set; }

    IReadOnlyDictionary<IBone, IBoneTracks> BoneTracks { get; }
    IBoneTracks AddBoneTracks(IBone bone);

    // TODO: Allow setting fps.
    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }

  public interface IBoneTracks {
    public ITrack<IPosition> Positions { get; }
    public ITrack<IRotation, Quaternion> Rotations { get; }
    public ITrack<IScale> Scales { get; }

    // TODO: Should these be null if empty?
  }

  // TODO: Add a track for animating params, e.g. textures, UVs, frames.

  public interface ITrack<T> : ITrack<T, T> {}

  public interface ITrack<TValue, out TInterpolated> {
    void Set(int frame, TValue t);

    TValue? GetAtFrame(int frame);
    TInterpolated? GetInterpolatedAtFrame(float frame);

    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }
}
