using fin.data;
using fin.math.interpolation;
using System.Collections.Generic;

namespace fin.model.impl {
  public partial class ModelImpl {
    private class TrackImpl<T> : TrackImpl<T, T>, ITrack<T> {
      public TrackImpl(
          int initialCapacity,
          IInterpolator<T> interpolator,
          IInterpolatorWithTangents<T> interpolatorWithTangent) :
          base(initialCapacity, interpolator, interpolatorWithTangent) { }
    }

    private class TrackImpl<TValue, TInterpolated> :
        ITrack<TValue, TInterpolated> {
      private readonly Keyframes<ValueAndTangents<TValue>> impl_;

      public TrackImpl(
          int initialCapacity,
          IInterpolator<TValue, TInterpolated> interpolator,
          IInterpolatorWithTangents<TValue, TInterpolated>
              interpolatorWithTangent) {
        this.impl_ = new Keyframes<ValueAndTangents<TValue>>(initialCapacity);
        this.Interpolator = interpolator;
        this.InterpolatorWithTangents = interpolatorWithTangent;
      }

      public IInterpolator<TValue, TInterpolated> Interpolator {
        get;
        private set;
      }

      public IInterpolatorWithTangents<TValue, TInterpolated>
          InterpolatorWithTangents { get; private set; }

      public int FrameCount { get; set; }

      public IReadOnlyList<Keyframe<ValueAndTangents<TValue>>> Keyframes
        => this.impl_.Definitions;

      public bool IsDefined => this.impl_.IsDefined;

      public void Set(ITrack<TValue, TInterpolated> other) {
        this.Interpolator = other.Interpolator;
        this.InterpolatorWithTangents = other.InterpolatorWithTangents;

        foreach (var keyframe in other.Keyframes) {
          this.Set(keyframe.Frame,
                   keyframe.Value.Value,
                   keyframe.Value.IncomingTangent,
                   keyframe.Value.OutgoingTangent);
        }
      }

      public void Set(
        int frame,
        TValue t,
        float? optionalIncomingTangent,
        float? optionalOutgoingTangent)
        => this.impl_.SetKeyframe(frame,
          new ValueAndTangents<TValue>(t, optionalIncomingTangent,
            optionalOutgoingTangent));

      public Keyframe<ValueAndTangents<TValue>>? GetKeyframe(int frame)
        => this.impl_.GetKeyframeAtFrame(frame);

      public TInterpolated GetInterpolatedFrame(
          float frame,
          TInterpolated defaultValue,
          bool useLoopingInterpolation = false) {
        var keyframeDefined = this.impl_.FindIndexOfKeyframe((int)frame,
          out var fromKeyframeIndex,
          out var fromKeyframeOrNull,
          out var isLastKeyframe);

        if (!keyframeDefined) {
          return defaultValue;
        }

        var fromKeyframe = fromKeyframeOrNull.Value;
        var fromValue = fromKeyframe.Value.Value;

        // TODO: Make this an option?
        if (isLastKeyframe && !useLoopingInterpolation) {
          return this.Interpolator.Interpolate(fromValue, fromValue, 0);
        }

        var fromTime = fromKeyframe.Frame;
        var fromTangent = fromKeyframe.Value.OutgoingTangent;

        var wrapsAround = isLastKeyframe && useLoopingInterpolation;

        var toKeyframe = !wrapsAround
                             ? this.impl_.GetKeyframeAtIndex(fromKeyframeIndex + 1)
                             : this.impl_.GetKeyframeAtIndex(0);
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
        return !useTangents
                ? this.Interpolator.Interpolate(fromValue, toValue, progress)
                : this.InterpolatorWithTangents.Interpolate(
                    fromTime,
                    fromValue,
                    fromTangent.Value,
                    toTime,
                    toValue,
                    toTangent.Value,
                    frame);
      }

      public bool GetInterpolationData(
          float frame,
          TValue defaultValue,
          out (float frame, TValue value, float? tangent)? fromData,
          out (float frame, TValue value, float? tangent)? toData,
          bool useLoopingInterpolation = false
      ) {
        var keyframeDefined = this.impl_.FindIndexOfKeyframe((int)frame,
                                 out var fromKeyframeIndex,
                                 out var fromKeyframeOrNull,
                                 out var isLastKeyframe);
        fromData = toData = null;

        if (!keyframeDefined) {
          return false;
        }

        var fromKeyframe = fromKeyframeOrNull.Value;
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
                ? this.impl_.GetKeyframeAtIndex(fromKeyframeIndex + 1)
                : this.impl_.GetKeyframeAtIndex(0);
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
  }
}