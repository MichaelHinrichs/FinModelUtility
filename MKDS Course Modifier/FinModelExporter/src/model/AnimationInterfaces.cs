using System.Collections.Generic;

namespace fin.model {
  public interface IAnimations {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();
  }

  public interface IAnimation {
    string Name { get; }
    IReadOnlyDictionary<IBone, IBoneTracks> BoneTracks { get; }
  }

  public interface IBoneTracks {
    // TODO: Track keyframes.
  }

  // TODO: Add a class for representing a sparsely populated track.
  // TODO: Add a track for animating params, e.g. textures, UVs, frames.
}
