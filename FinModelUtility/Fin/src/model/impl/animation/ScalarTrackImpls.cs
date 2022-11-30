using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using fin.math.interpolation;
using fin.util.optional;


namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrackImpl : IPositionTrack {
      private readonly ScalarAxesTrack<IPosition, float> impl_ =
          new(3,
              Optional.Of<float>(0),
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              axisList => new PositionImpl {
                  X = axisList[0],
                  Y = axisList[1],
                  Z = axisList[2],
              });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, IPosition> other)
        => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          Optional<float> optionalIncomingTangent,
          Optional<float> optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Optional<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IPosition GetInterpolatedFrame(
          float frame,
          IOptional<float[]>? defaultAxes = null,
          bool useLoopingInterpolation = false)
        => this.impl_.GetInterpolatedFrame(
            frame,
            defaultAxes,
            useLoopingInterpolation);
    }

    public class ScaleTrackImpl : IScaleTrack {
      private readonly ScalarAxesTrack<IScale, float> impl_ =
          new(3,
              Optional.Of<float>(1),
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              axes
                  => new ScaleImpl {
                      X = axes[0],
                      Y = axes[1],
                      Z = axes[2],
                  });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, IScale> other) => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          Optional<float> optionalIncomingTangent,
          Optional<float> optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Optional<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public IScale GetInterpolatedFrame(
          float frame,
          IOptional<float[]>? defaultAxes = null,
          bool useLoopingInterpolation = false
      )
        => this.impl_.GetInterpolatedFrame(frame, defaultAxes,
                                           useLoopingInterpolation);
    }


    public class TrackImpl<T> : TrackImpl<T, T>, ITrack<T> {
      public TrackImpl(
          IInterpolator<T> interpolator,
          IInterpolatorWithTangents<T> interpolatorWithTangent) :
          base(interpolator, interpolatorWithTangent) { }
    }

    public class TrackImpl<TValue, TInterpolated> :
        ITrack<TValue, TInterpolated> {
      private readonly IList<Keyframe<TValue>> keyframesAndValues_ =
          new List<Keyframe<TValue>>();

      public TrackImpl(
          IInterpolator<TValue, TInterpolated> interpolator,
          IInterpolatorWithTangents<TValue, TInterpolated>
              interpolatorWithTangent) {
        this.Interpolator = interpolator;
        this.InterpolatorWithTangents = interpolatorWithTangent;
        this.Keyframes =
            new ReadOnlyCollection<Keyframe<TValue>>(
                this.keyframesAndValues_);
      }

      public IInterpolator<TValue, TInterpolated> Interpolator {
        get;
        private set;
      }

      public IInterpolatorWithTangents<TValue, TInterpolated>
          InterpolatorWithTangents { get; private set; }

      public int FrameCount { get; set; }
      public IReadOnlyList<Keyframe<TValue>> Keyframes { get; }

      public bool IsDefined { get; set; }

      public void Set(ITrack<TValue, TInterpolated> other) {
        this.Interpolator = other.Interpolator;
        this.InterpolatorWithTangents = other.InterpolatorWithTangents;

        foreach (var keyframe in other.Keyframes) {
          this.Set(keyframe.Frame,
                   keyframe.Value,
                   keyframe.IncomingTangent,
                   keyframe.OutgoingTangent);
        }
      }

      public void Set(
          int frame,
          TValue t,
          Optional<float> optionalIncomingTangent,
          Optional<float> optionalOutgoingTangent) {
        this.IsDefined = true;

        this.FindIndexOfKeyframe(frame,
                                 out var keyframeIndex,
                                 out var maybeKeyframe,
                                 out var keyframeDefined,
                                 out var isLastKeyframe);
        var keyframeAndValue =
            new Keyframe<TValue>(frame,
                                 t,
                                 optionalIncomingTangent,
                                 optionalOutgoingTangent);
        if (maybeKeyframe.Try(out var keyframe) &&
            keyframe.Frame == frame) {
          this.keyframesAndValues_[keyframeIndex] = keyframeAndValue;
        } else if (isLastKeyframe) {
          this.keyframesAndValues_.Add(keyframeAndValue);
        } else if (keyframeDefined) {
          this.keyframesAndValues_[keyframeIndex] = keyframeAndValue;
        } else {
          this.keyframesAndValues_.Insert(keyframeIndex, keyframeAndValue);
        }
      }

      public Optional<Keyframe<TValue>> GetKeyframe(int frame) {
        this.FindIndexOfKeyframe(frame,
                                 out var keyframeIndex,
                                 out var value,
                                 out _,
                                 out _);
        if (value.Try(out var keyframe) && keyframe.Frame == frame) {
          return value;
        }
        return Optional.None<Keyframe<TValue>>();
      }

      public Optional<TInterpolated> GetInterpolatedFrame(
          float frame,
          IOptional<TValue> defaultValue,
          bool useLoopingInterpolation = false) {
        this.FindIndexOfKeyframe((int) frame,
                                 out var fromKeyframeIndex,
                                 out var optionalFromKeyframe,
                                 out var keyframeDefined,
                                 out var isLastKeyframe);

        var hasFromValue = optionalFromKeyframe
                           .Pluck(keyframe => keyframe.Value)
                           .Or(defaultValue)
                           .Try(out var fromValue);

        if (!hasFromValue) {
          return Optional.None<TInterpolated>();
        }

        // TODO: Make this an option?
        if (!keyframeDefined || (isLastKeyframe && !useLoopingInterpolation)) {
          return Optional.Of(
              this.Interpolator.Interpolate(fromValue, fromValue, 0));
        }

        var fromKeyframe =
            optionalFromKeyframe.Assert("Keyframe should be defined here!");
        var fromTime = fromKeyframe.Frame;
        var hasFromTangent =
            fromKeyframe.OutgoingTangent.Try(out var fromTangent);

        var wrapsAround = isLastKeyframe && useLoopingInterpolation;

        var toKeyframe = !wrapsAround
                             ? this.keyframesAndValues_[fromKeyframeIndex + 1]
                             : this.keyframesAndValues_[0];
        var toValue = toKeyframe.Value;
        var toTime = toKeyframe.Frame;

        if (wrapsAround) {
          if (frame >= fromTime) {
            toTime += this.FrameCount;
          } else {
            fromTime -= this.FrameCount;
          }
        }

        var hasToTangent = toKeyframe.IncomingTangent.Try(out var toTangent);

        var duration = toTime - fromTime;
        var progress = (frame - fromTime) / duration;

        var useTangents = hasFromTangent && hasToTangent;
        return Optional.Of(
            !useTangents
                ? this.Interpolator.Interpolate(fromValue, toValue, progress)
                : this.InterpolatorWithTangents.Interpolate(
                    fromTime,
                    fromValue,
                    fromTangent,
                    toTime,
                    toValue,
                    toTangent,
                    frame));
      }

      public bool GetInterpolationData(
          float frame,
          IOptional<TValue> defaultValue,
          out (float frame, TValue value, IOptional<float> tangent)? fromData,
          out (float frame, TValue value, IOptional<float> tangent)? toData,
          bool useLoopingInterpolation = false
      ) {
        this.FindIndexOfKeyframe((int) frame,
                                 out var fromKeyframeIndex,
                                 out var optionalFromKeyframe,
                                 out var keyframeDefined,
                                 out var isLastKeyframe);
        fromData = toData = null;

        var hasFromValue = optionalFromKeyframe
                           .Pluck(keyframe => keyframe.Value)
                           .Try(out var fromValue);

        if (!hasFromValue) {
          return false;
        }

        var fromKeyframe =
            optionalFromKeyframe.Assert("Keyframe should be defined here!");
        var fromTime = fromKeyframe.Frame;

        fromData = (fromTime, fromValue, fromKeyframe.OutgoingTangent);

        // TODO: Make this an option?
        if (!keyframeDefined || (isLastKeyframe && !useLoopingInterpolation)) {
          return true;
        }

        var wrapsAround = isLastKeyframe && useLoopingInterpolation;

        var toKeyframe =
            !wrapsAround
                ? this.keyframesAndValues_[fromKeyframeIndex + 1]
                : this.keyframesAndValues_[0];
        var toTime = toKeyframe.Frame;
        var toValue = toKeyframe.Value;

        if (wrapsAround) {
          if (frame >= fromTime) {
            toTime += this.FrameCount;
          } else {
            fromTime -= this.FrameCount;
            fromData = (fromTime, fromValue, fromKeyframe.OutgoingTangent);
          }
        }

        toData = (toTime, toValue, toKeyframe.IncomingTangent);
        return true;
      }

      // TODO: Use a more efficient approach here, e.g. binary search.
      public void FindIndexOfKeyframe(
          int frame,
          out int keyframeIndex,
          out Optional<Keyframe<TValue>> keyframe,
          out bool keyframeDefined,
          out bool isLastKeyframe) {
        var keyframeCount = this.keyframesAndValues_.Count;
        for (var i = keyframeCount - 1; i >= 0; --i) {
          var currentKeyframe = this.keyframesAndValues_[i];

          if (currentKeyframe.Frame <= frame) {
            keyframeIndex = i;
            keyframe = Optional.Of(currentKeyframe);
            keyframeDefined = true;
            isLastKeyframe = i == keyframeCount - 1;
            return;
          }
        }

        keyframeIndex = keyframeCount;
        keyframe = Optional.None<Keyframe<TValue>>();
        keyframeDefined = false;
        isLastKeyframe = true;
      }
    }


    public class
        ScalarAxesTrack<TAxes, TAxis> : ScalarAxesTrack<TAxes, TAxis, TAxes> {
      public ScalarAxesTrack(
          int axisCount,
          Optional<TAxis> defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated)
          : base(axisCount,
                 defaultValue,
                 axisInterpolator,
                 axisInterpolatorWithTangent,
                 mergeAxisListIntoInterpolated) { }
    }

    public class ScalarAxesTrack<TAxes, TAxis, TInterpolated> :
        IAxesTrack<TAxis, TInterpolated> {
      private readonly Optional<TAxis> defaultValue_;

      public delegate TInterpolated MergeAxisListIntoInterpolated(
          TAxis[] axisList);

      private TrackImpl<TAxis>[] axisTracks_;

      private readonly MergeAxisListIntoInterpolated
          mergeAxisListIntoInterpolated_;

      public ScalarAxesTrack(
          int axisCount,
          Optional<TAxis> defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          MergeAxisListIntoInterpolated mergeAxisListIntoInterpolated) {
        this.axisTracks_ = new TrackImpl<TAxis>[axisCount];
        for (var i = 0; i < axisCount; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<TAxis>(axisInterpolator,
                                   axisInterpolatorWithTangent);
        }

        this.AxisTracks =
            new ReadOnlyCollection<ITrack<TAxis>>(this.axisTracks_);

        this.defaultValue_ = defaultValue;

        this.mergeAxisListIntoInterpolated_ = mergeAxisListIntoInterpolated;
      }

      public bool IsDefined => this.axisTracks_.Any(axis => axis.IsDefined);

      public int FrameCount {
        set {
          foreach (var axis in this.axisTracks_) {
            axis.FrameCount = value;
          }
        }
      }

      public void Set(IAxesTrack<TAxis, TInterpolated> other) {
        var otherAxisTracks = other.AxisTracks;
        for (var i = 0; i < otherAxisTracks.Count; ++i) {
          this.axisTracks_[i].Set(otherAxisTracks[i]);
        }
      }

      public void Set(
          int frame,
          int axis,
          TAxis value,
          Optional<float> optionalIncomingTangent,
          Optional<float> optionalOutgoingTangent)
        => this.axisTracks_[axis]
               .Set(frame,
                    value,
                    optionalIncomingTangent,
                    optionalOutgoingTangent);

      public Optional<Keyframe<TAxis>> GetKeyframe(int keyframe, int axis)
        => this.axisTracks_[axis].GetKeyframe(keyframe);


      public IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }

      public Optional<Keyframe<TAxis>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public TInterpolated GetInterpolatedFrame(
          float frame,
          IOptional<TAxis[]>? defaultAxes = null,
          bool useLoopingInterpolation = false
      ) {
        var optionAxisList =
            this.axisTracks_.Select(
                (axis, i) => {
                  var defaultValue = defaultAxes == null
                                         ? this.defaultValue_
                                         : defaultAxes.Pluck(axes => axes[i])
                                             .Or(this.defaultValue_);

                  return axis.GetInterpolatedFrame(
                      frame,
                      defaultValue,
                      useLoopingInterpolation);
                });
        var axisList = optionAxisList
                       .Select(axis => axis.Assert(
                                   "Could not interpolate value for one of the axes!"))
                       .ToArray();

        return this.mergeAxisListIntoInterpolated_(axisList);
      }
    }
  }
}