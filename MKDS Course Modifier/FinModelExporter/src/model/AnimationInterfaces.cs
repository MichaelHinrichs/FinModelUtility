using System.Collections.Generic;

namespace fin.model {
  public interface IAnimationManager {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();
  }

  public interface IAnimation {
    string Name { get; }
    IReadOnlyDictionary<IBone, IBoneTracks?> BoneTracks { get; }

    // TODO: Allow setting fps.
    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }

  public interface IBoneTracks {
    public ITrack<IPosition>? Positions { get; }
    public ITrack<IQuaternion>? Rotations { get; }
    public ITrack<IScale>? Scales { get; }

    // TODO: Add pattern for specifying WITH given tracks
  }

  // TODO: Add a track for animating params, e.g. textures, UVs, frames.

  public interface ITrack<T> {
    void Set(int frame, T t);

    T? GetAtFrame(int frame);
    T? GetInterpolatedAtFrame(float frame);

    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }
}
