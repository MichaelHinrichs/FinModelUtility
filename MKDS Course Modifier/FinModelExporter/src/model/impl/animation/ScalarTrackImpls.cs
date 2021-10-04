using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using fin.util.asserts;

using Optional;
using Optional.Unsafe;

namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrackImpl : IPositionTrack {
      private readonly ScalarAxesTrack<IPosition, float> impl_ =
          new(3,
              Option.Some<float>(0),
              TrackInterpolators.Float,
              TrackInterpolators.FloatWithTangents,
              axisList => new PositionImpl {
                  X = axisList[0],
                  Y = axisList[1],
                  Z = axisList[2],
              });

      public void Set(int frame, int axis, float value)
        => this.impl_.Set(frame, axis, value);

      public void Set(int frame, int axis, float value, float tangent)
        => this.impl_.Set(frame, axis, value, tangent);

      public Option<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IPosition GetInterpolatedFrame(float frame)
        => this.impl_.GetInterpolatedFrame(frame);
    }

    public class ScaleTrackImpl : IScaleTrack {
      private readonly ScalarAxesTrack<IScale, float> impl_ =
          new(3,
              Option.Some<float>(1),
              TrackInterpolators.Float,
              TrackInterpolators.FloatWithTangents,
              axes
                  => new ScaleImpl {
                      X = axes[0],
                      Y = axes[1],
                      Z = axes[2],
                  });

      public void Set(int frame, int axis, float value)
        => this.impl_.Set(frame, axis, value);

      public void Set(int frame, int axis, float value, float tangent)
        => this.impl_.Set(frame, axis, value, tangent);

      public Option<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IScale GetInterpolatedFrame(float frame)
        => this.impl_.GetInterpolatedFrame(frame);
    }


    public class TrackImpl<T> : TrackImpl<T, T>, ITrack<T> {
      public TrackImpl(
          InterpolateValues interpolator,
          InterpolateValuesWithTangent interpolatorWithTangent) :
          base(interpolator, interpolatorWithTangent) {}
    }

    public class TrackImpl<TValue, TInterpolated> :
        ITrack<TValue, TInterpolated> {
      public delegate TInterpolated InterpolateValues(
          TValue fromValue,
          TValue toValue,
          float progress);

      public delegate TInterpolated InterpolateValuesWithTangent(
          TValue fromValue,
          float fromTangent,
          TValue toValue,
          float toTangent,
          float progress,
          float length);

      public readonly InterpolateValues interpolator_;
      public readonly InterpolateValuesWithTangent interpolatorWithTangent_;

      private readonly IList<Keyframe<TValue>> keyframesAndValues_ =
          new List<Keyframe<TValue>>();

      public TrackImpl(
          InterpolateValues interpolator,
          InterpolateValuesWithTangent interpolatorWithTangent) {
        this.interpolator_ = interpolator;
        this.interpolatorWithTangent_ = interpolatorWithTangent;
        this.Keyframes =
            new ReadOnlyCollection<Keyframe<TValue>>(
                this.keyframesAndValues_);
      }

      public IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

      public void Set(int frame, TValue t)
        => this.SetImpl_(frame, t, Option.None<float>());

      public void Set(int frame, TValue t, float tangent)
        => this.SetImpl_(frame, t, Option.Some(tangent));

      private void SetImpl_(int frame, TValue t, Option<float> tangent) {
        this.FindIndexOfKeyframe(frame,
                                 out var keyframeIndex,
                                 out _,
                                 out var keyframeDefined,
                                 out var pastEnd);
        var keyframeAndValue = new Keyframe<TValue>(frame, t, tangent);
        if (pastEnd) {
          this.keyframesAndValues_.Add(keyframeAndValue);
        } else if (keyframeDefined) {
          this.keyframesAndValues_[keyframeIndex] = keyframeAndValue;
        } else {
          this.keyframesAndValues_.Insert(keyframeIndex, keyframeAndValue);
        }
      }

      public Option<Keyframe<TValue>> GetKeyframe(int frame) {
        this.FindIndexOfKeyframe(frame,
                                 out var keyframeIndex,
                                 out var value,
                                 out _,
                                 out _);
        if (this.keyframesAndValues_[keyframeIndex].Frame == frame) {
          return value;
        }
        return Option.None<Keyframe<TValue>>();
      }

      public Option<TInterpolated> GetInterpolatedFrame(
          float frame,
          Option<TValue> defaultValue) {
        this.FindIndexOfKeyframe((int) frame,
                                 out var fromKeyframeIndex,
                                 out var optionalFromKeyframe,
                                 out var keyframeDefined,
                                 out var pastEnd);

        var optionalFromValue = optionalFromKeyframe.HasValue
                                    ? Option.Some(
                                        optionalFromKeyframe.ValueOrFailure()
                                            .Value)
                                    : defaultValue;

        if (!keyframeDefined && !defaultValue.HasValue) {
          return Option.None<TInterpolated>();
        }

        var fromValue = optionalFromValue.ValueOrFailure();
        var hasFromTangent = optionalFromKeyframe.HasValue &&
                             optionalFromKeyframe.ValueOrFailure()
                                                 .Tangent.HasValue;
        var isLastKeyframe =
            fromKeyframeIndex == this.keyframesAndValues_.Count - 1;

        // TODO: Make this an option?
        if (!keyframeDefined ||
            pastEnd ||
            isLastKeyframe) {
          return Option.Some(
              this.interpolator_(fromValue, fromValue, 0));
        }

        var (toKeyframeTime, toValue, toTangent) =
            this.keyframesAndValues_[fromKeyframeIndex + 1];

        var fromKeyframe = optionalFromKeyframe.ValueOrFailure();
        var fromKeyframeTime = fromKeyframe.Frame;

        var length = toKeyframeTime - fromKeyframeTime;
        var progress = (frame - fromKeyframeTime) / length;

        // TODO: Unfortunately, linear interpolation is way more accurate right
        // now. What's going wrong here??
        var useTangents = hasFromTangent && toTangent.HasValue;
        return Option.Some(!useTangents
                               ? this.interpolator_(fromValue!,
                                                    toValue,
                                                    progress)
                               : this.interpolatorWithTangent_(
                                   fromValue!,
                                   optionalFromKeyframe.ValueOrFailure()
                                       .Tangent.ValueOrFailure(),
                                   toValue,
                                   toTangent.ValueOrFailure(),
                                   progress,
                                   length));
      }

      // TODO: Use a more efficient approach here, e.g. binary search.
      public void FindIndexOfKeyframe(
          int frame,
          out int keyframeIndex,
          out Option<Keyframe<TValue>> keyframe,
          out bool keyframeDefined,
          out bool pastEnd) {
        var keyframeCount = this.keyframesAndValues_.Count;
        for (var i = keyframeCount - 1; i >= 0; --i) {
          var currentKeyframe = this.keyframesAndValues_[i];

          if (currentKeyframe.Frame <= frame) {
            keyframeIndex = i;
            keyframe = Option.Some(currentKeyframe);
            keyframeDefined = true;
            pastEnd = i == keyframeCount - 1;
            return;
          }
        }

        keyframeIndex = keyframeCount;
        keyframe = Option.None<Keyframe<TValue>>();
        keyframeDefined = false;
        pastEnd = true;
      }
    }


    public class
        ScalarAxesTrack<TAxes, TAxis> : ScalarAxesTrack<TAxes, TAxis, TAxes> {
      public ScalarAxesTrack(
          int axisCount,
          Option<TAxis> defaultValue,
          TrackImpl<TAxis>.InterpolateValues axisInterpolator,
          TrackImpl<TAxis>.InterpolateValuesWithTangent
              axisInterpolatorWithTangent,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated)
          : base(axisCount,
                 defaultValue,
                 axisInterpolator,
                 axisInterpolatorWithTangent,
                 mergeAxisListIntoInterpolated) {}
    }

    public class ScalarAxesTrack<TAxes, TAxis, TInterpolated> :
        IAxesTrack<TAxis, TInterpolated> {
      private readonly Option<TAxis> defaultValue_;

      public delegate TInterpolated MergeAxisListIntoInterpolated(
          TAxis[] axisList);

      private readonly TrackImpl<TAxis>[] axisTracks_;

      private readonly MergeAxisListIntoInterpolated
          mergeAxisListIntoInterpolated_;

      public ScalarAxesTrack(
          int axisCount,
          Option<TAxis> defaultValue,
          TrackImpl<TAxis>.InterpolateValues axisInterpolator,
          TrackImpl<TAxis>.InterpolateValuesWithTangent
              axisInterpolatorWithTangent,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated) {
        this.axisTracks_ = new TrackImpl<TAxis>[axisCount];
        for (var i = 0; i < axisCount; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<TAxis>(axisInterpolator,
                                   axisInterpolatorWithTangent);
        }

        this.defaultValue_ = defaultValue;

        this.mergeAxisListIntoInterpolated_ = mergeAxisListIntoInterpolated;
      }

      public void Set(int frame, int axis, TAxis value)
        => this.axisTracks_[axis].Set(frame, value);

      public void Set(int frame, int axis, TAxis value, float tangent)
        => this.axisTracks_[axis].Set(frame, value, tangent);

      public Option<Keyframe<TAxis>> GetKeyframe(int keyframe, int axis)
        => this.axisTracks_[axis].GetKeyframe(keyframe);

      public Option<Keyframe<TAxis>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public TInterpolated GetInterpolatedFrame(float frame) {
        var optionAxisList =
            this.axisTracks_.Select(
                axis => axis.GetInterpolatedFrame(frame, this.defaultValue_));
        var axisList = optionAxisList
                       .Select(axis => axis.ValueOrFailure(
                                   "Could not interpolate value for one of the axes!"))
                       .ToArray();

        return this.mergeAxisListIntoInterpolated_(axisList);
      }
    }
  }
}