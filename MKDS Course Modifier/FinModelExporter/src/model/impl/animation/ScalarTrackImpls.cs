using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Optional;
using Optional.Unsafe;

namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrackImpl : IPositionTrack {
      private readonly ScalarAxesTrack<IPosition, float> impl_ =
          new(3,
              Option.Some<float>(0),
              TrackInterpolators.FloatInterpolator,
              axisList => new PositionImpl {
                  X = axisList[0],
                  Y = axisList[1],
                  Z = axisList[2],
              });

      public void Set(int frame, int axis, float value)
        => this.impl_.Set(frame, axis, value);

      public Option<float>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IPosition GetInterpolatedFrame(float frame)
        => this.impl_.GetInterpolatedFrame(frame);
    }

    public class ScaleTrackImpl : IScaleTrack {
      private readonly ScalarAxesTrack<IScale, float> impl_ =
          new(3,
              Option.Some<float>(1),
              TrackInterpolators.FloatInterpolator,
              axes
                  => new ScaleImpl {
                      X = axes[0],
                      Y = axes[1],
                      Z = axes[2],
                  });

      public void Set(int frame, int axis, float value)
        => this.impl_.Set(frame, axis, value);

      public Option<float>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IScale GetInterpolatedFrame(float frame)
        => this.impl_.GetInterpolatedFrame(frame);
    }


    public class TrackImpl<T> : TrackImpl<T, T>, ITrack<T> {
      public TrackImpl(Func<T, T, float, T> interpolator) :
          base(interpolator) {}
    }

    public class TrackImpl<TValue, TInterpolated> :
        ITrack<TValue, TInterpolated> {
      public readonly Func<TValue, TValue, float, TInterpolated>
          interpolator_;

      private readonly IList<Keyframe<TValue>> keyframesAndValues_ =
          new List<Keyframe<TValue>>();

      public TrackImpl(
          Func<TValue, TValue, float, TInterpolated> interpolator) {
        this.interpolator_ = interpolator;
        this.Keyframes =
            new ReadOnlyCollection<Keyframe<TValue>>(
                this.keyframesAndValues_);
      }

      public IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

      public void Set(int frame, TValue t) {
        this.FindIndexOfKeyframe(frame,
                                 out var keyframeIndex,
                                 out _,
                                 out var keyframeDefined,
                                 out var pastEnd,
                                 Option.None<TValue>());
        var keyframeAndValue = new Keyframe<TValue>(frame, t);
        if (pastEnd) {
          this.keyframesAndValues_.Add(keyframeAndValue);
        } else if (keyframeDefined) {
          this.keyframesAndValues_[keyframeIndex] = keyframeAndValue;
        } else {
          this.keyframesAndValues_.Insert(keyframeIndex, keyframeAndValue);
        }
      }

      public Option<TValue> GetKeyframe(int frame) {
        this.FindIndexOfKeyframe(frame,
                                 out _,
                                 out var value,
                                 out var keyframeDefined,
                                 out _,
                                 Option.None<TValue>());

        return keyframeDefined ? value : Option.None<TValue>();
      }

      public Option<TInterpolated> GetInterpolatedFrame(
          float frame,
          Option<TValue> defaultValue) {
        this.FindIndexOfKeyframe((int) frame,
                                 out var fromKeyframeIndex,
                                 out var optionalFromValue,
                                 out var keyframeDefined,
                                 out var pastEnd,
                                 defaultValue);

        if (!keyframeDefined && !optionalFromValue.HasValue) {
          return Option.None<TInterpolated>();
        }

        var fromValue = optionalFromValue.ValueOrFailure();
        var isLastKeyframe =
            fromKeyframeIndex == this.keyframesAndValues_.Count - 1;

        // TODO: Make this an option?
        if (!keyframeDefined ||
            pastEnd ||
            isLastKeyframe) {
          return Option.Some(
              this.interpolator_(fromValue, fromValue, 0));
        }

        var (toKeyframeIndex, toValue) =
            this.keyframesAndValues_[fromKeyframeIndex + 1];

        return Option.Some(this.interpolator_(fromValue!,
                                              toValue,
                                              (frame - fromKeyframeIndex) /
                                              (toKeyframeIndex -
                                               fromKeyframeIndex)));
      }

      // TODO: Use a more efficient approach here, e.g. binary search.
      public void FindIndexOfKeyframe(
          int frame,
          out int keyframeIndex,
          out Option<TValue> value,
          out bool keyframeDefined,
          out bool pastEnd,
          Option<TValue> defaultValue) {
        var keyframeCount = this.keyframesAndValues_.Count;
        for (var i = keyframeCount - 1; i >= 0; --i) {
          var (currentKeyframe, t) = this.keyframesAndValues_[i];

          if (currentKeyframe <= frame) {
            keyframeIndex = i;
            value = Option.Some(t);
            keyframeDefined = true;
            pastEnd = i == keyframeCount - 1;
            return;
          }
        }

        keyframeIndex = keyframeCount;
        value = defaultValue;
        keyframeDefined = false;
        pastEnd = true;
      }
    }


    public class
        ScalarAxesTrack<TAxes, TAxis> : ScalarAxesTrack<TAxes, TAxis, TAxes> {
      public ScalarAxesTrack(
          int axisCount,
          Option<TAxis> defaultValue,
          Func<TAxis, TAxis, float, TAxis> axisInterpolator,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated)
          : base(axisCount,
                 defaultValue,
                 axisInterpolator,
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

      private readonly IList<Keyframe<TAxes>> keyframesAndValues_ =
          new List<Keyframe<TAxes>>();

      public ScalarAxesTrack(
          int axisCount,
          Option<TAxis> defaultValue,
          Func<TAxis, TAxis, float, TAxis> axisInterpolator,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated) {
        this.axisTracks_ = new TrackImpl<TAxis>[axisCount];
        for (var i = 0; i < axisCount; ++i) {
          this.axisTracks_[i] = new TrackImpl<TAxis>(axisInterpolator);
        }

        this.defaultValue_ = defaultValue;

        this.mergeAxisListIntoInterpolated_ = mergeAxisListIntoInterpolated;
      }

      public void Set(int frame, int axis, TAxis value)
        => this.axisTracks_[axis].Set(frame, value);

      public Option<TAxis> GetKeyframe(int keyframe, int axis) {
        this.axisTracks_[axis]
            .FindIndexOfKeyframe(keyframe,
                                 out var _,
                                 out var value,
                                 out var keyframeDefined,
                                 out var _,
                                 Option.None<TAxis>());
        return keyframeDefined ? value : Option.None<TAxis>();
      }

      public Option<TAxis>[] GetAxisListAtKeyframe(int keyframe)
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