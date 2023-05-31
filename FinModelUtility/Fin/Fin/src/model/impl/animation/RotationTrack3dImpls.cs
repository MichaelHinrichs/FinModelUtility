using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using fin.data;
using fin.math;
using fin.math.interpolation;


namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public class RadiansRotationTrack3dImpl : IEulerRadiansRotationTrack3d {
      private readonly IBone bone_;
      private readonly IInputOutputTrack<float, RadianInterpolator>[]
          axisTracks_;

      public RadiansRotationTrack3dImpl(
          IBone bone,
          ReadOnlySpan<int> initialCapacityPerAxis) {
        this.bone_ = bone;
        this.axisTracks_ = new InputOutputTrackImpl<float, RadianInterpolator>[3];
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i] = new InputOutputTrackImpl<float, RadianInterpolator>
              (initialCapacityPerAxis[i], new RadianInterpolator());
        }
      }

      public bool IsDefined => this.axisTracks_.Any(axis => axis.IsDefined);

      public int FrameCount {
        set {
          foreach (var axis in this.axisTracks_) {
            axis.FrameCount = value;
          }
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
          bool useLoopingInterpolation = false) {
        var xTrack = this.axisTracks_[0];
        var yTrack = this.axisTracks_[1];
        var zTrack = this.axisTracks_[2];

        var localRotation = this.bone_.LocalRotation;
        var defaultX = localRotation?.XRadians ?? 0;
        var defaultY = localRotation?.YRadians ?? 0;
        var defaultZ = localRotation?.ZRadians ?? 0;

        xTrack.TryGetInterpolationData(
            frame,
            out var fromXFrame,
            out var toXFrame,
            useLoopingInterpolation);
        yTrack.TryGetInterpolationData(
            frame,
            out var fromYFrame,
            out var toYFrame,
            useLoopingInterpolation);
        zTrack.TryGetInterpolationData(
            frame,
            out var fromZFrame,
            out var toZFrame,
            useLoopingInterpolation);

        Span<(float frame, float value, float? tangent)?> fromsAndTos =
            stackalloc (float frame, float value, float? tangent)?[6];
        fromsAndTos[0] = fromXFrame;
        fromsAndTos[1] = fromYFrame;
        fromsAndTos[2] = fromZFrame;
        fromsAndTos[3] = toXFrame;
        fromsAndTos[4] = toYFrame;
        fromsAndTos[5] = toZFrame;

        Span<bool> areAxesStatic = stackalloc bool[3];
        RadiansRotationTrack3dImpl.AreAxesStatic_(fromsAndTos, areAxesStatic);
        
        if (!RadiansRotationTrack3dImpl.CanInterpolateWithQuaternions_(
                fromsAndTos, areAxesStatic)) {
          if (!xTrack.TryGetInterpolatedFrame(frame,
                                              out var xRadians,
                                              useLoopingInterpolation)) {
            xRadians = defaultX;
          }
          if (!yTrack.TryGetInterpolatedFrame(frame,
                                              out var yRadians,
                                              useLoopingInterpolation)) {
            yRadians = defaultY;
          }
          if (!zTrack.TryGetInterpolatedFrame(frame,
                                              out var zRadians,
                                              useLoopingInterpolation)) {
            zRadians = defaultZ;
          }

          return ConvertRadiansToQuaternionImpl(xRadians, yRadians, zRadians);
        }

        if (RadiansRotationTrack3dImpl.GetFromAndToFrameIndex_(fromsAndTos,
              areAxesStatic,
              out var fromFrame,
              out var toFrame)) {
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

        return Quaternion.Normalize(ConvertRadiansToQuaternionImpl(
            fromXFrame?.value ?? defaultX,
            fromYFrame?.value ?? defaultY,
            fromZFrame?.value ?? defaultZ));
      }

      private static void AreAxesStatic_(
          ReadOnlySpan<(float frame, float value, float? tangent)?> fromsAndTos,
          Span<bool> areAxesStatic) {
        for (var i = 0; i < 3; ++i) {
          var from = fromsAndTos[i];
          var to = fromsAndTos[3 + i];

          if (from == null && to == null) {
            areAxesStatic[i] = true;
          } else if (from != null && to != null) {
            areAxesStatic[i] =
                Math.Abs(from.Value.value - to.Value.value) < .0001;
          }
        }
      }

      private static bool GetFromAndToFrameIndex_(
          ReadOnlySpan<(float frame, float value, float? tangent)?> fromsAndTos,
          ReadOnlySpan<bool> areAxesStatic,
          out float fromFrameIndex,
          out float toFrameIndex) {
        for (var i = 0; i < 3; ++i) {
          if (!areAxesStatic[i]) {
            fromFrameIndex = fromsAndTos[i].Value.frame;
            toFrameIndex = fromsAndTos[3 + i].Value.frame;
            return true;
          }
        }

        fromFrameIndex = 0;
        toFrameIndex = 1;
        return false;
      }

      // TODO: Might be able to use this for euler-interpolated keyframe i and i+1
      private static bool CanInterpolateWithQuaternions_(
          ReadOnlySpan<(float frame, float value, float? tangent)?> fromsAndTos,
          ReadOnlySpan<bool> areAxesStatic) {
        for (var i = 0; i < 6; ++i) {
          if (areAxesStatic[i % 3]) {
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
          if (areAxesStatic[i]) {
            continue;
          }

          for (var oi = i + 1; oi < 3; ++oi) {
            if (areAxesStatic[oi]) {
              continue;
            }

            if (fromsAndTos[i].Value.frame != fromsAndTos[oi].Value.frame) {
              return false;
            }
            if (fromsAndTos[3 + i].Value.frame !=
                fromsAndTos[3 + oi].Value.frame) {
              return false;
            }
          }
        }

        return true;
      }

      public IEulerRadiansRotationTrack3d.ConvertRadiansToQuaternion
          ConvertRadiansToQuaternionImpl { get; set; } =
        QuaternionUtil.CreateZyx;
    }
  }
}