using fin.util.optional;

using System.Collections.Generic;
using System.Numerics;

using fin.math.interpolation;

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
    public void Set(IBoneTracks other);

    // TODO: Should these be null if empty?
    public IPositionTrack Positions { get; }
    public IRadiansRotationTrack Rotations { get; }
    public IScaleTrack Scales { get; }
  }

  // TODO: Add a track for animating params, e.g. textures, UVs, frames.
  public record Keyframe<T>(int Frame, T Value, Optional<float> Tangent);


  public interface ITrack<T> : ITrack<T, T> {}

  public interface ITrack<TValue, TInterpolated> {
    public IInterpolator<TValue, TInterpolated> Interpolator { get; }

    public IInterpolatorWithTangents<TValue, TInterpolated>
        InterpolatorWithTangents { get; }

    IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

    void Set(ITrack<TValue, TInterpolated> other);
    void Set(int frame, TValue t);
    void Set(int frame, TValue t, float tangent);
    void Set(int frame, TValue t, Optional<float> optionalTangent);

    Optional<Keyframe<TValue>> GetKeyframe(int frame);

    Optional<TInterpolated> GetInterpolatedFrame(
        float frame,
        Optional<TValue> defaultValue);

    // TODO: Allow setting tangent(s) at each frame.
    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }

  // TODO: Rethink this, this is getting way too complicated.
  public interface IAxesTrack<TAxis, TInterpolated> {
    void Set(IAxesTrack<TAxis, TInterpolated> other);

    void Set(int frame, int axis, TAxis value);
    void Set(int frame, int axis, TAxis value, float tangent);
    void Set(int frame, int axis, TAxis value, Optional<float> optionalTangent);

    IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }
    Optional<Keyframe<TAxis>>[] GetAxisListAtKeyframe(int keyframe);

    TInterpolated GetInterpolatedFrame(float frame);
  }

  public interface IPositionTrack : IAxesTrack<float, IPosition> {}

  public interface IRadiansRotationTrack : IAxesTrack<float, Quaternion> {
    // TODO: Document this, better name
    public IRotation GetAlmostKeyframe(float frame);
  }

  public interface IScaleTrack : IAxesTrack<float, IScale> {}
}