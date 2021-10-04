using System.Linq;
using System.Numerics;

using fin.math;
using fin.util.asserts;

using Optional;
using Optional.Unsafe;

namespace fin.model.impl {
  public partial class ModelImpl {
    public class RadiansRotationTrackImpl : IRadiansRotationTrack {
      private readonly Option<float> defaultRotation_ = Option.Some<float>(0);
      private readonly TrackImpl<float>[] axisTracks_;

      public RadiansRotationTrackImpl() {
        this.axisTracks_ = new TrackImpl<float>[3];
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<float>(TrackInterpolators.Float,
                                   TrackInterpolators.FloatWithTangents);
        }
      }

      public void Set(int frame, int axis, float radians)
        => this.axisTracks_[axis].Set(frame, radians);

      public void Set(int frame, int axis, float radians, float tangent)
        => this.axisTracks_[axis].Set(frame, radians, tangent);

      public Option<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public IRotation GetAlmostKeyframe(float frame)
        => new RotationImpl().SetRadians(
            this.axisTracks_[0]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .ValueOrFailure(),
            this.axisTracks_[1]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .ValueOrFailure(),
            this.axisTracks_[2]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .ValueOrFailure());

      public Quaternion GetInterpolatedFrame(float frame) {
        var xTrack = this.axisTracks_[0];
        var yTrack = this.axisTracks_[1];
        var zTrack = this.axisTracks_[2];

        var keyframe = (int) frame;

        // TODO: Properly interpolate between first and final keyframe
        xTrack.FindIndexOfKeyframe(keyframe,
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

        var xRadians = xRadiansKeyframe.HasValue
                           ? xRadiansKeyframe.ValueOrFailure().Value
                           : this.defaultRotation_.ValueOrFailure();
        var yRadians = yRadiansKeyframe.HasValue
                           ? yRadiansKeyframe.ValueOrFailure().Value
                           : this.defaultRotation_.ValueOrFailure();
        var zRadians = zRadiansKeyframe.HasValue
                           ? zRadiansKeyframe.ValueOrFailure().Value
                           : this.defaultRotation_.ValueOrFailure();

        return QuaternionUtil.Create(xRadians, yRadians, zRadians);
      }
    }
  }
}