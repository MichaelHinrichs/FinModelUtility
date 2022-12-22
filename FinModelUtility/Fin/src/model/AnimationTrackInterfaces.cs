using fin.data;
using fin.math.interpolation;
using fin.util.optional;
using System.Collections.Generic;
using System.Numerics;


namespace fin.model {
  public record ValueAndTangents<T>(
      T Value,
      Optional<float> IncomingTangent,
      Optional<float> OutgoingTangent);


  public interface ITrack<T> : ITrack<T, T> { }

  public interface ITrack<TValue, TInterpolated> {
    bool IsDefined { get; }

    int FrameCount { get; set; }

    IInterpolator<TValue, TInterpolated> Interpolator { get; }

    IInterpolatorWithTangents<TValue, TInterpolated>
       InterpolatorWithTangents { get; }

    IReadOnlyList<Keyframe<ValueAndTangents<TValue>>> Keyframes { get; }

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

    Optional<Keyframe<ValueAndTangents<TValue>>> GetKeyframe(int frame);

    Optional<TInterpolated> GetInterpolatedFrame(
        float frame,
        IOptional<TValue> defaultValue,
        bool useLoopingInterpolation = false
    );

    bool GetInterpolationData(
        float frame,
        IOptional<TValue> defaultValue,
        out (float frame, TValue value, IOptional<float> tangent)? fromData,
        out (float frame, TValue value, IOptional<float> tangent)? toData,
        bool useLoopingInterpolation = false
    );

    // TODO: Allow setting tangent(s) at each frame.
    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }

  // TODO: Rethink this, this is getting way too complicated.
  public interface IAxesTrack<TAxis, TInterpolated> {
    bool IsDefined { get; }

    int FrameCount { set; }

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
    Optional<Keyframe<ValueAndTangents<TAxis>>>[] GetAxisListAtKeyframe(int keyframe);

    TInterpolated GetInterpolatedFrame(
        float frame,
        IOptional<TAxis[]>? defaultValue = null,
        bool useLoopingInterpolation = false
    );
  }

  public interface IPositionTrack : IAxesTrack<float, IPosition> { }

  public interface IRadiansRotationTrack : IAxesTrack<float, Quaternion> {
  }

  public interface IScaleTrack : IAxesTrack<float, IScale> { }
}
