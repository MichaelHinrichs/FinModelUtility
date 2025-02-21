﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

using fin.math.interpolation;
using fin.model;
using fin.util.asserts;

namespace fin.animation {
  public readonly record struct ValueAndTangents<T>(
      T IncomingValue,
      T OutgoingValue,
      float? IncomingTangent,
      float? OutgoingTangent) {
    public ValueAndTangents(T value) : this(value, null) { }

    public ValueAndTangents(T value, float? tangent)
        : this(value, tangent, tangent) { }

    public ValueAndTangents(T value,
                            float? incomingTangent,
                            float? outgoingTangent)
        : this(value,
               value,
               incomingTangent,
               outgoingTangent) { }
  }


  public interface ITrack : IAnimationData {
    bool HasAtLeastOneKeyframe { get; }
    int MaxKeyframe { get; }
  }

  public readonly record struct AnimationInterpolationConfig {
    public bool UseLoopingInterpolation { get; init; }
  }

  public interface IReadOnlyInterpolatedTrack<TInterpolated> : ITrack {
    bool TryGetInterpolatedFrame(
        float frame,
        out TInterpolated interpolatedValue,
        AnimationInterpolationConfig? config = null
    );

    TInterpolated GetInterpolatedFrame(
        float frame,
        AnimationInterpolationConfig? config = null
    ) {
      Asserts.True(this.TryGetInterpolatedFrame(frame, out var interpolatedValue, config));
      return interpolatedValue;
    }
  }


  public interface IImplTrack<TValue> : ITrack {
    IReadOnlyList<Keyframe<ValueAndTangents<TValue>>> Keyframes { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SetKeyframe(int frame, TValue value, string frameType = "")
      => this.SetKeyframe(frame, value, null, frameType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SetKeyframe(int frame, TValue value, float? tangent, string frameType = "")
      => this.SetKeyframe(frame, value, tangent, tangent, frameType);

    void SetKeyframe(
        int frame,
        TValue value,
        float? incomingTangent,
        float? outgoingTangent,
        string frameType = "")
      => this.SetKeyframe(frame,
                          value,
                          value,
                          incomingTangent,
                          outgoingTangent,
                          frameType);

    void SetKeyframe(
        int frame,
        TValue incomingValue,
        TValue outgoingValue,
        float? incomingTangent,
        float? outgoingTangent,
        string frameType = "");

    void SetAllKeyframes(IEnumerable<TValue> value);

    bool TryGetInterpolationData(
        float frame,
        out (float frame, TValue value, float? tangent)? fromData,
        out (float frame, TValue value, float? tangent)? toData,
        AnimationInterpolationConfig? config = null
    );

    Keyframe<ValueAndTangents<TValue>>? GetKeyframe(int frame);
  }


  public interface IInputOutputTrack<T, TInterpolator>
      : IInputOutputTrack<T, T, TInterpolator>
      where TInterpolator : IInterpolator<T> { }

  public interface IInputOutputTrack<TValue, TInterpolated, TInterpolator>
      : IReadOnlyInterpolatedTrack<TInterpolated>,
        IImplTrack<TValue>
      where TInterpolator : IInterpolator<TValue, TInterpolated> {
    TInterpolator Interpolator { get; }

    void Set(IInputOutputTrack<TValue, TInterpolated, TInterpolator> other) { }

    // TODO: Allow setting tangent(s) at each frame.
    // TODO: Allow setting easing at each frame.
    // TODO: Split getting into exactly at frame and interpolated at frame.
    // TODO: Allow getting at fractional frames.
  }

  // TODO: Rethink this, this is getting way too complicated.
  public interface IAxesTrack<in TAxis, TInterpolated>
      : IReadOnlyInterpolatedTrack<TInterpolated> {
    void Set(int frame, int axis, TAxis value, string frameType = "")
      => this.Set(frame, axis, value, null, frameType);

    void Set(int frame, int axis, TAxis value, float? optionalTangent, string frameType = "")
      => this.Set(frame, axis, value, optionalTangent, optionalTangent, frameType);

    void Set(
        int frame,
        int axis,
        TAxis value,
        float? optionalIncomingTangent,
        float? optionalOutgoingTangent,
        string frameType = "")
      => this.Set(frame,
                  axis,
                  value,
                  value,
                  optionalIncomingTangent,
                  optionalOutgoingTangent,
                  frameType);

    void Set(
        int frame,
        int axis,
        TAxis incomingValue,
        TAxis outgoingValue,
        float? optionalIncomingTangent,
        float? optionalOutgoingTangent,
        string frameType = "");
  }
}