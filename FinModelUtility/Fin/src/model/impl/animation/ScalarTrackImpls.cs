using fin.data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using fin.math.interpolation;
using fin.util.optional;
using schema.util;


namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrackImpl : IPositionTrack {
      private readonly ScalarAxesTrack<PositionStruct, float> impl_ =
          new(3,
              Optional.Of<float>(0),
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue) => new PositionStruct {
                  X = axesTrack.AxisTracks[0].GetInterpolatedFrame(frame).Or(defaultValue[0]),
                  Y = axesTrack.AxisTracks[1].GetInterpolatedFrame(frame).Or(defaultValue[1]),
                  Z = axesTrack.AxisTracks[2].GetInterpolatedFrame(frame).Or(defaultValue[2]),
              });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, PositionStruct> other)
        => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Optional<Keyframe<ValueAndTangents<float>>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public PositionStruct GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false)
        => this.impl_.GetInterpolatedFrame(
            frame,
            defaultValue,
            useLoopingInterpolation);
    }

    public class ScaleTrackImpl : IScaleTrack {
      private readonly ScalarAxesTrack<ScaleStruct, float> impl_ =
          new(3,
              Optional.Of<float>(1),
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue)
                  => new ScaleStruct {
                    X = axesTrack.AxisTracks[0].GetInterpolatedFrame(frame).Or(defaultValue[0]),
                    Y = axesTrack.AxisTracks[1].GetInterpolatedFrame(frame).Or(defaultValue[1]),
                    Z = axesTrack.AxisTracks[2].GetInterpolatedFrame(frame).Or(defaultValue[2]),
                  });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, ScaleStruct> other) => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Optional<Keyframe<ValueAndTangents<float>>>[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public ScaleStruct GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false
      )
        => this.impl_.GetInterpolatedFrame(frame, defaultValue,
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
      private readonly Keyframes<ValueAndTangents<TValue>> impl_ = new();

      public TrackImpl(
          IInterpolator<TValue, TInterpolated> interpolator,
          IInterpolatorWithTangents<TValue, TInterpolated>
              interpolatorWithTangent) {
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

      public Optional<Keyframe<ValueAndTangents<TValue>>> GetKeyframe(int frame)
        => this.impl_.GetKeyframeAtFrame(frame);

      public Optional<TInterpolated> GetInterpolatedFrame(
          float frame,
          bool useLoopingInterpolation = false) {
        var keyframeDefined = this.impl_.FindIndexOfKeyframe((int)frame,
          out var fromKeyframeIndex,
          out var optionalFromKeyframe,
          out var isLastKeyframe);

        var hasFromValue = optionalFromKeyframe
          .Pluck(keyframe => keyframe.Value.Value)
          .Try(out var fromValue);

        if (!hasFromValue) {
          return Optional.None<TInterpolated>();
        }

        // TODO: Make this an option?
        if (!keyframeDefined || (isLastKeyframe && !useLoopingInterpolation)) {
          return Optional.Of(
              this.Interpolator.Interpolate(fromValue, fromValue, 0));
        }

        var fromKeyframe = optionalFromKeyframe.Assert();
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
        return Optional.Of(
            !useTangents
                ? this.Interpolator.Interpolate(fromValue, toValue, progress)
                : this.InterpolatorWithTangents.Interpolate(
                    fromTime,
                    fromValue,
                    fromTangent.Value,
                    toTime,
                    toValue,
                    toTangent.Value,
                    frame));
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
                                 out var optionalFromKeyframe,
                                 out var isLastKeyframe);
        fromData = toData = null;

        var hasFromValue = optionalFromKeyframe
          .Pluck(keyframe => keyframe.Value.Value)
          .Try(out var fromValue);

        if (!hasFromValue) {
          return false;
        }

        var fromKeyframe = optionalFromKeyframe.Assert();
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


    public class ScalarAxesTrack<TAxes, TAxis> :
          ScalarAxesTrack<TAxes, TAxis, TAxes> {
      public ScalarAxesTrack(
          int axisCount,
          Optional<TAxis> defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          GetInterpolatedFromAxesTrack getInterpolatedFromAxesTrack)
          : base(axisCount,
                 defaultValue,
                 axisInterpolator,
                 axisInterpolatorWithTangent,
                 getInterpolatedFromAxesTrack) { }
    }

    public class ScalarAxesTrack<TAxes, TAxis, TInterpolated> :
        IAxesTrack<TAxis, TInterpolated> {
      private readonly Optional<TAxis> defaultValue_;

      public delegate TInterpolated GetInterpolatedFromAxesTrack(
          IAxesTrack<TAxis, TInterpolated> axesTrack,
          float frame,
          TAxis[] defaultValue);

      private TrackImpl<TAxis>[] axisTracks_;

      private readonly GetInterpolatedFromAxesTrack
          getInterpolatedFromAxesTrack_;

      public ScalarAxesTrack(
          int axisCount,
          Optional<TAxis> defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          GetInterpolatedFromAxesTrack getInterpolatedFromAxesTrack) {
        this.axisTracks_ = new TrackImpl<TAxis>[axisCount];
        for (var i = 0; i < axisCount; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<TAxis>(axisInterpolator,
                                   axisInterpolatorWithTangent);
        }

        this.AxisTracks =
            new ReadOnlyCollection<ITrack<TAxis>>(this.axisTracks_);

        this.defaultValue_ = defaultValue;

        this.getInterpolatedFromAxesTrack_ = getInterpolatedFromAxesTrack;
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
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.axisTracks_[axis]
               .Set(frame,
                    value,
                    optionalIncomingTangent,
                    optionalOutgoingTangent);

      public Optional<Keyframe<ValueAndTangents<TAxis>>> GetKeyframe(int keyframe, int axis)
        => this.axisTracks_[axis].GetKeyframe(keyframe);


      public IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }

      public Optional<Keyframe<ValueAndTangents<TAxis>>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();


      public TInterpolated GetInterpolatedFrame(
          float frame,
          TAxis[] defaultValue,
          bool useLoopingInterpolation = false
      ) {
        return this.getInterpolatedFromAxesTrack_(this, frame, defaultValue);
      }
    }
  }
}