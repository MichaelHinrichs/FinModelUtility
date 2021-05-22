using System.Collections.Generic;

namespace fin.model {
  public interface IAnimations {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();
  }

  public interface IAnimation {
    string Name { get; }
    IReadOnlyDictionary<IBone, IBoneTracks> BoneTracks { get; }

    // TODO: Allow setting fps.
    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }

  public interface IBoneTracks {
    public ITrack<IPosition> Positions { get; }
    public ITrack<IQuaternion> Rotations { get; }
    public ITrack<IScale> Scales { get; }
  }

  // TODO: Add a track for animating params, e.g. textures, UVs, frames.

  public interface ITrack<T> {
    T this[int frame] { get; set; }

    // TODO: Allow setting easing at each frame.
  }
}
