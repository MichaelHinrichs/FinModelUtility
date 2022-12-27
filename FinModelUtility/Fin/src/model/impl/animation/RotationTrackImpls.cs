using fin.data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

using fin.math;
using fin.math.interpolation;
using fin.util.optional;


namespace fin.model.impl {
  public partial class ModelImpl {
    public class RadiansRotationTrackImpl : IRadiansRotationTrack {
      private readonly Optional<float> defaultRotation_ = Optional.Of<float>(0);
      private readonly TrackImpl<float>[] axisTracks_;

      public RadiansRotationTrackImpl() {
        this.axisTracks_ = new TrackImpl<float>[3];
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<float>(Interpolator.Float,
                                   InterpolatorWithTangents.Radians);
        }

        this.AxisTracks =
            new ReadOnlyCollection<ITrack<float>>(this.axisTracks_);
      }

      public IReadOnlyList<ITrack<float>> AxisTracks { get; }

      public bool IsDefined => this.axisTracks_.Any(axis => axis.IsDefined);

      public int FrameCount {
        set {
          foreach (var axis in this.axisTracks_) {
            axis.FrameCount = value;
          }
        }
      }

      public void Set(IAxesTrack<float, Quaternion> other) {
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i].Set(other.AxisTracks[i]);
        }
      }

      public void Set(
          int frame,
          int axis,
          float radians,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.axisTracks_[axis]
               .Set(frame,
                    radians,
                    optionalIncomingTangent,
                    optionalOutgoingTangent);

      public Optional<Keyframe<ValueAndTangents<float>>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public Quaternion GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false) {
        var xTrack = this.axisTracks_[0];
        var yTrack = this.axisTracks_[1];
        var zTrack = this.axisTracks_[2];

        var keyframe = (int)frame;

        // TODO: Properly interpolate between first and final keyframe
        // TODO: Fix gimbal lock
        /*xTrack.FindIndexOfKeyframe(keyframe,
                                   out var xKeyframeIndex,
                                   out var xRadiansKeyframe,
                                   out var xKeyframeDefined,
                                   out var xPastEnd);
        yTrack.FindIndexOfKeyframe(keyframe,
                                   out var yKeyframeIndex,
                                   out var yRadiansKeyframe,
                                   out var yKeyframeDefined,
                                   out var yPastEnd);
        zTrack.FindIndexOfKeyframe(keyframe,
                                   out var zKeyframeIndex,
                                   out var zRadiansKeyframe,
                                   out var zKeyframeDefined,
                                   out var zPastEnd);

        var fromXRadians = xRadiansKeyframe.Pluck(keyframe => keyframe.Value)
                                       .Or(this.defaultRotation_)
                                       .Assert();
        var fromYRadians = yRadiansKeyframe.Pluck(keyframe => keyframe.Value)
                                       .Or(this.defaultRotation_)
                                       .Assert();
        var fromZRadians = zRadiansKeyframe.Pluck(keyframe => keyframe.Value)
                                       .Or(this.defaultRotation_)
                                       .Assert();

        var xKeyframes = this.axisTracks_[0].Keyframes;
        if (xKeyframeIndex < xKeyframes.Count) {

        }*/

        var defaultX = defaultValue[0];
        var defaultY = defaultValue[1];
        var defaultZ = defaultValue[2];

        xTrack.GetInterpolationData(
            frame,
            defaultX,
            out var fromXFrame,
            out var toXFrame,
            useLoopingInterpolation);
        yTrack.GetInterpolationData(
            frame,
            defaultY,
            out var fromYFrame,
            out var toYFrame,
            useLoopingInterpolation);
        zTrack.GetInterpolationData(
            frame,
            defaultZ,
            out var fromZFrame,
            out var toZFrame,
            useLoopingInterpolation);

        if (!RadiansRotationTrackImpl.CanInterpolateWithQuaternions_(
                fromXFrame, fromYFrame, fromZFrame,
                toXFrame, toYFrame, toZFrame)) {
          var xRadians =
              xTrack.GetInterpolatedFrame(frame, useLoopingInterpolation)
                    .Or(defaultX);
          var yRadians =
              yTrack.GetInterpolatedFrame(frame, useLoopingInterpolation)
                    .Or(defaultY);
          var zRadians =
              zTrack.GetInterpolatedFrame(frame, useLoopingInterpolation)
                    .Or(defaultZ);

          return QuaternionUtil.Create(xRadians, yRadians, zRadians);
        }

        var fromFrame = fromXFrame.Value.frame;
        var toFrame = toXFrame.Value.frame;
        var frameDelta = (frame - fromFrame) / (toFrame - fromFrame);

        var q1 = QuaternionUtil.Create(
            fromXFrame.Value.value,
            fromYFrame.Value.value,
            fromZFrame.Value.value);
        var q2 = QuaternionUtil.Create(
            toXFrame.Value.value,
            toYFrame.Value.value,
            toZFrame.Value.value);

        if (Quaternion.Dot(q1, q2) < 0) {
          q2 = -q2;
        }

        var interp = Quaternion.Slerp(q1, q2, frameDelta);
        return Quaternion.Normalize(interp);
      }

      // TODO: Might be able to use this for euler-interpolated keyframe i and i+1
      private static bool CanInterpolateWithQuaternions_(
          params (float frame, float value, float? tangent)?[]
              fromsAndTos) {
        if (fromsAndTos.Any(frameData => {
              if (frameData == null) {
                return true;
              }

              // TODO: Use tangents if all fromFrames have the same tangent and all
              // toFrames have the same tangent.
              return (frameData.Value.tangent ?? 0) != 0;
            })) {
          return false;
        }

        var firstFromFrame = fromsAndTos[0].Value.frame;
        if (fromsAndTos.Skip(1)
                       .Take(2)
                       .Any(frame => frame.Value.frame != firstFromFrame)) {
          return false;
        }

        var firstToFrame = fromsAndTos[3].Value.frame;
        if (fromsAndTos.Skip(3)
                       .Skip(1)
                       .Take(2)
                       .Any(frame =>
                                frame.Value.frame != firstToFrame)) {
          return false;
        }

        return true;
      }
    }
  }
}