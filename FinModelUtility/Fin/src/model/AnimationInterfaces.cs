using fin.util.optional;

using System.Collections.Generic;
using System.Numerics;

using fin.data;
using fin.math.interpolation;

namespace fin.model {
  public interface IAnimationManager {
    IReadOnlyList<IAnimation> Animations { get; }
    IAnimation AddAnimation();

    IReadOnlyList<IMorphTarget> MorphTargets { get; }
    IMorphTarget AddMorphTarget();
  }

  public interface IMorphTarget {
    string Name { get; set; }

    IReadOnlyDictionary<IVertex, IPosition> Morphs { get; }
    IMorphTarget MoveTo(IVertex vertex, IPosition position);
  }

  public interface IAnimation {
    string Name { get; set; }

    int FrameCount { get; set; }
    float FrameRate { get; set; }

    IReadOnlyIndexableDictionary<IBone, IBoneTracks> BoneTracks { get; }
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
  public record Keyframe<T>(
      int Frame,
      T Value,
      Optional<float> IncomingTangent,
      Optional<float> OutgoingTangent);


  public interface ITrack<T> : ITrack<T, T> {}

  public interface ITrack<TValue, TInterpolated> {
    bool IsDefined { get; }

    public IInterpolator<TValue, TInterpolated> Interpolator { get; }

    public IInterpolatorWithTangents<TValue, TInterpolated>
        InterpolatorWithTangents { get; }

    IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

    void Set(ITrack<TValue, TInterpolated> other);

    void Set(int frame, TValue value)
      => this.Set(frame, value, Optional.None<float>());

    void Set(int frame, TValue value, float tangent)
      => this.Set(frame, value, tangent, tangent);

    void Set(int frame, TValue value, Optional<float> optionalTangent)
      => this.Set(frame, value, optionalTangent, optionalTangent);

    void Set(
        int frame,
        TValue value,
        float incomingTangent,
        float outgoingTangent)
      => this.Set(frame,
                  value,
                  Optional.Of(incomingTangent),
                  Optional.Of(outgoingTangent));

    void Set(
        int frame,
        TValue value,
        Optional<float> optionalIncomingTangent,
        Optional<float> optionalOutgoingTangent);

    Optional<Keyframe<TValue>> GetKeyframe(int frame);

    Optional<TInterpolated> GetInterpolatedFrame(
        float frame,
        IOptional<TValue> defaultValue);

    // TODO: Allow setting tangent(s) at each frame.
    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }

  // TODO: Rethink this, this is getting way too complicated.
  public interface IAxesTrack<TAxis, TInterpolated> {
    bool IsDefined { get; }

    void Set(IAxesTrack<TAxis, TInterpolated> other);

    void Set(int frame, int axis, TAxis value)
      => this.Set(frame, axis, value, Optional.None<float>());

    void Set(int frame, int axis, TAxis value, float tangent)
      => this.Set(frame, axis, value, tangent, tangent);

    void Set(int frame, int axis, TAxis value, Optional<float> optionalTangent)
      => this.Set(frame, axis, value, optionalTangent, optionalTangent);

    void Set(
        int frame,
        int axis,
        TAxis value,
        float incomingTangent,
        float outgoingTangent)
      => this.Set(frame,
                  axis,
                  value,
                  Optional.Of(incomingTangent),
                  Optional.Of(outgoingTangent));

    void Set(
        int frame,
        int axis,
        TAxis value,
        Optional<float> optionalIncomingTangent,
        Optional<float> optionalOutgoingTangent);


    IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }
    Optional<Keyframe<TAxis>>[] GetAxisListAtKeyframe(int keyframe);

    TInterpolated GetInterpolatedFrame(
        float frame,
        IOptional<TAxis[]>? defaultValue = null);
  }

  public interface IPositionTrack : IAxesTrack<float, IPosition> {}

  public interface IRadiansRotationTrack : IAxesTrack<float, Quaternion> {
    // TODO: Document this, better name
    public IRotation GetAlmostKeyframe(float frame);
  }

  public interface IScaleTrack : IAxesTrack<float, IScale> {}
}