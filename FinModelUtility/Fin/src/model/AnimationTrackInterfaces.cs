using fin.data;
using fin.math.interpolation;
using System.Collections.Generic;


namespace fin.model {
  public readonly struct ValueAndTangents<T> {
    public T Value { get; }
    public float? IncomingTangent { get; }
    public float? OutgoingTangent { get; }

    public ValueAndTangents(T value,
                            float? incomingTangent,
                            float? outgoingTangent) {
      this.Value = value;
      this.IncomingTangent = incomingTangent;
      this.OutgoingTangent = outgoingTangent;
    }
  }

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
      => this.Set(frame, value, null);

    void Set(int frame, TValue value, float? tangent)
      => this.Set(frame, value, tangent, tangent);

    void Set(
        int frame,
        TValue value,
        float? incomingTangent,
        float? outgoingTangent);

    Keyframe<ValueAndTangents<TValue>>? GetKeyframe(int frame);

    TInterpolated GetInterpolatedFrame(
        float frame,
        TInterpolated defaultValue,
        bool useLoopingInterpolation = false
    );

    bool GetInterpolationData(
        float frame,
        TValue defaultValue,
        out (float frame, TValue value, float? tangent)? fromData,
        out (float frame, TValue value, float? tangent)? toData,
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
      => this.Set(frame, axis, value, null);

    void Set(int frame, int axis, TAxis value, float? optionalTangent)
      => this.Set(frame, axis, value, optionalTangent, optionalTangent);

    void Set(
        int frame,
        int axis,
        TAxis value,
        float? optionalIncomingTangent,
        float? optionalOutgoingTangent);


    IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }
    Keyframe<ValueAndTangents<TAxis>>?[] GetAxisListAtKeyframe(int keyframe);

    TInterpolated GetInterpolatedFrame(
        float frame,
        TAxis[] defaultValue,
        bool useLoopingInterpolation = false
    );
  }
}
