using System;

using fin.data;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using fin.math;
using fin.math.interpolation;


namespace fin.model.impl {
  public partial class ModelImpl {
    public class RadiansRotationTrack3dImpl : IRadiansRotationTrack3d {
      private readonly TrackImpl<float>[] axisTracks_;

      public RadiansRotationTrack3dImpl(
          ReadOnlySpan<int> initialCapacityPerAxis) {
        this.axisTracks_ = new TrackImpl<float>[3];
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<float>(
                  initialCapacityPerAxis[i],
                  Interpolator.Float,
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

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Keyframe<ValueAndTangents<float>>?[] GetAxisListAtKeyframe(
          int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public Quaternion GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false) {
        var xTrack = this.axisTracks_[0];
        var yTrack = this.axisTracks_[1];
        var zTrack = this.axisTracks_[2];

        var keyframe = (int) frame;

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

        if (!RadiansRotationTrack3dImpl.CanInterpolateWithQuaternions_(
                fromXFrame,
                fromYFrame,
                fromZFrame,
                toXFrame,
                toYFrame,
                toZFrame)) {
          var xRadians =
              xTrack.GetInterpolatedFrame(frame,
                                          defaultX,
                                          useLoopingInterpolation);
          var yRadians =
              yTrack.GetInterpolatedFrame(frame,
                                          defaultY,
                                          useLoopingInterpolation);
          var zRadians =
              zTrack.GetInterpolatedFrame(frame,
                                          defaultZ,
                                          useLoopingInterpolation);

          return ConvertRadiansToQuaternionImpl(xRadians, yRadians, zRadians);
        }

        var fromFrame = fromXFrame?.frame ?? fromYFrame?.frame ?? fromZFrame.Value.frame;
        var toFrame = toXFrame?.frame ?? toYFrame?.frame ?? toZFrame.Value.frame;
        var frameDelta = (frame - fromFrame) / (toFrame - fromFrame);

        var q1 = ConvertRadiansToQuaternionImpl(
            fromXFrame?.value ?? defaultX,
            fromYFrame?.value ?? defaultY,
            fromZFrame?.value ?? defaultZ);
        var q2 = ConvertRadiansToQuaternionImpl(
            toXFrame?.value ?? defaultX,
            toYFrame?.value ?? defaultY,
            toZFrame?.value ?? defaultZ);

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
        Span<bool> okAxes = stackalloc bool[3];
        for (var i = 0; i < 3; ++i) {
          var from = fromsAndTos[i];
          var to = fromsAndTos[3 + i];

          okAxes[i] = (from == null && to == null) ||
                      Math.Abs(from.Value.value - to.Value.value) < .0001;
        }

        for (var i = 0; i < 6; ++i) {
          if (okAxes[i % 3]) {
            continue;
          }

          if (fromsAndTos[i] == null) {
            return false;
          }

          // TODO: Use tangents if all fromFrames have the same tangent and all
          // toFrames have the same tangent.
          if ((fromsAndTos[i].Value.tangent ?? 0) != 0) {
            return false;
          }
        }

        for (var i = 0; i < 3; ++i) {
          if (okAxes[i]) {
            continue;
          }

          for (var oi = 0; oi < 3; ++oi) {
            if (oi == i || okAxes[oi]) {
              continue;
            }

            if (fromsAndTos[i].Value.frame != fromsAndTos[oi].Value.frame) {
              return false;
            }
            if (fromsAndTos[3 + i].Value.frame != fromsAndTos[3 + oi].Value.frame) {
              return false;
            }
          }
        }

        return true;
      }

      public IRadiansRotationTrack3d.ConvertRadiansToQuaternion
          ConvertRadiansToQuaternionImpl { get; set; } =
        QuaternionUtil.CreateZyx;
    }
  }
}