using fin.data;
using fin.math.interpolation;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public class ImplTrackImpl<TValue> : IImplTrack<TValue> {
      protected readonly Keyframes<ValueAndTangents<TValue>> impl;

      public ImplTrackImpl(int initialCapacity) {
        this.impl = new Keyframes<ValueAndTangents<TValue>>(initialCapacity);
      }

      public int FrameCount { get; set; }

      public IReadOnlyList<Keyframe<ValueAndTangents<TValue>>> Keyframes
        => this.impl.Definitions;

      public bool IsDefined => this.impl.IsDefined;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Set(
          int frame,
          TValue t,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.impl.SetKeyframe(frame,
                                  new ValueAndTangents<TValue>(t,
                                    optionalIncomingTangent,
                                    optionalOutgoingTangent));

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Keyframe<ValueAndTangents<TValue>>? GetKeyframe(int frame)
        => this.impl.GetKeyframeAtFrame(frame);

      public bool GetInterpolationData(
          float frame,
          TValue defaultValue,
          out (float frame, TValue value, float? tangent)? fromData,
          out (float frame, TValue value, float? tangent)? toData,
          bool useLoopingInterpolation = false
      ) {
        var keyframeDefined = this.impl.FindIndexOfKeyframe((int) frame,
          out var fromKeyframeIndex,
          out var fromKeyframe,
          out var isLastKeyframe);
        fromData = toData = null;

        if (!keyframeDefined) {
          return false;
        }

        var fromValue = fromKeyframe.Value.Value;

        var fromTime = fromKeyframe.Frame;
        var fromOutgoingTangent = fromKeyframe.Value.OutgoingTangent;
        fromData = (fromTime, fromValue, fromOutgoingTangent);

        // TODO: Make this an option?
        if (!keyframeDefined || (isLastKeyframe && !useLoopingInterpolation)) {
          return true;
        }

        var wrapsAround = isLastKeyframe && useLoopingInterpolation;

        var toKeyframe =
            !wrapsAround
                ? this.impl.GetKeyframeAtIndex(fromKeyframeIndex + 1)
                : this.impl.GetKeyframeAtIndex(0);
        var toTime = toKeyframe.Frame;
        var toValue = toKeyframe.Value.Value;
        var toIncomingTangent = toKeyframe.Value.IncomingTangent;

        if (wrapsAround) {
          if (frame >= fromTime) {
            toTime += this.FrameCount;
          } else {
            fromTime -= this.FrameCount;
            fromData = (fromTime, fromValue, fromOutgoingTangent);
          }
        }

        toData = (toTime, toValue, toIncomingTangent);
        return true;
      }
    }

    public class InputOutputTrackImpl<T, TInterpolator>
        : InputOutputTrackImpl<T, T, TInterpolator>,
          IInputOutputTrack<T, TInterpolator>
        where TInterpolator : IInterpolator<T> {
      public InputOutputTrackImpl(
          int initialCapacity,
          TInterpolator interpolator) :
          base(initialCapacity, interpolator) { }
    }

    public class InputOutputTrackImpl<TValue, TInterpolated, TInterpolator>
        : ImplTrackImpl<TValue>,
          IInputOutputTrack<TValue, TInterpolated, TInterpolator>
        where TInterpolator : IInterpolator<TValue, TInterpolated> {
      public InputOutputTrackImpl(
          int initialCapacity,
          TInterpolator interpolator) : base(initialCapacity) {
        this.Interpolator = interpolator;
      }

      public TInterpolator Interpolator { get; private set; }

      public void Set(
          IInputOutputTrack<TValue, TInterpolated, TInterpolator> other) {
        this.Interpolator = other.Interpolator;

        foreach (var keyframe in other.Keyframes) {
          this.Set(keyframe.Frame,
                   keyframe.Value.Value,
                   keyframe.Value.IncomingTangent,
                   keyframe.Value.OutgoingTangent);
        }
      }

      public bool TryGetInterpolatedFrame(
          float frame,
          out TInterpolated interpolatedValue,
          bool useLoopingInterpolation = false) {
        var keyframeDefined = this.impl.FindIndexOfKeyframe((int) frame,
          out var fromKeyframeIndex,
          out var fromKeyframe,
          out var isLastKeyframe);

        if (!keyframeDefined) {
          interpolatedValue = default;
          return false;
        }

        var fromValue = fromKeyframe.Value.Value;

        // TODO: Make this an option?
        if (isLastKeyframe && !useLoopingInterpolation) {
          interpolatedValue = this.Interpolator.Interpolate(fromValue, fromValue, 0);
          return true;
        }

        var fromTime = fromKeyframe.Frame;
        var fromTangent = fromKeyframe.Value.OutgoingTangent;

        var wrapsAround = isLastKeyframe && useLoopingInterpolation;

        var toKeyframe = !wrapsAround
            ? this.impl.GetKeyframeAtIndex(fromKeyframeIndex + 1)
            : this.impl.GetKeyframeAtIndex(0);
        var toValue = toKeyframe.Value.Value;
        var toTime = toKeyframe.Frame;

        if (wrapsAround) {
          if (frame >= fromTime) {
            toTime += this.FrameCount;
          } else {
            fromTime -= this.FrameCount;
          }
        }

        var toTangent = toKeyframe.Value.IncomingTangent;

        var duration = toTime - fromTime;
        var progress = (frame - fromTime) / duration;

        var useTangents = fromTangent != null && toTangent != null;
        interpolatedValue = !useTangents
            ? this.Interpolator.Interpolate(fromValue, toValue, progress)
            : this.Interpolator.Interpolate(
                fromTime,
                fromValue,
                fromTangent.Value,
                toTime,
                toValue,
                toTangent.Value,
                frame);
        return true;
      }
    }
  }
}