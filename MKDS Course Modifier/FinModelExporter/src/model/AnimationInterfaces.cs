using System;
using System.Collections.Generic;
using System.Numerics;

using Optional;

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

    // TODO: Allow setting looping behavior (once, back and forth, etc.)
  }

  public interface IBoneTracks {
    public IPositionTrack Positions { get; }
    public IRadiansRotationTrack Rotations { get; }
    public IScaleTrack Scales { get; }

    // TODO: Should these be null if empty?
  }

  // TODO: Add a track for animating params, e.g. textures, UVs, frames.

  public interface ITrack<T> : ITrack<T, T> {}

  public record Keyframe<T>(int Frame, T Value);

  public interface ITrack<TValue, TInterpolated> {
    IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

    void Set(int frame, TValue t);

    Option<TValue> GetKeyframe(int frame);

    Option<TInterpolated> GetInterpolatedFrame(
        float frame,
        Option<TValue> defaultValue);

    // TODO: Allow setting tangent(s) at each frame.
    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }

  // TODO: Rethink this, this is getting way too complicated.
  public interface IAxesTrack<TAxis, TInterpolated> {
    void Set(int frame, int axis, TAxis value);

    Option<TAxis>[] GetAxisListAtKeyframe(int keyframe);

    TInterpolated GetInterpolatedFrame(float frame);
  }

  public interface IPositionTrack : IAxesTrack<float, IPosition> {}

  public interface IRadiansRotationTrack : IAxesTrack<float, Quaternion> {
    // TODO: Document this
    public IRotation GetAlmostKeyframe(float frame);
  }

  public interface IScaleTrack : IAxesTrack<float, IScale> {}
}