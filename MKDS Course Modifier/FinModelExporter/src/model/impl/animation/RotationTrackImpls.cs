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
              new TrackImpl<float>(TrackInterpolators.FloatInterpolator);
        }
      }

      public void Set(int frame, int axis, float radians)
        => this.axisTracks_[axis].Set(frame, radians);

      public Option<float>[] GetAxisListAtKeyframe(int keyframe)
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
                                   out var xRadians,
                                   out var xKeyframeDefined,
                                   out var xPastEnd,
                                   this.defaultRotation_);
        yTrack.FindIndexOfKeyframe(keyframe,
                                   out var yKeyframeIndex,
                                   out var yRadians,
                                   out var yKeyframeDefined,
                                   out var yPastEnd,
                                   this.defaultRotation_);
        zTrack.FindIndexOfKeyframe(keyframe,
                                   out var zKeyframeIndex,
                                   out var zRadians,
                                   out var zKeyframeDefined,
                                   out var zPastEnd,
                                   this.defaultRotation_);

        return QuaternionUtil.Create(xRadians.ValueOrFailure(),
                                     yRadians.ValueOrFailure(),
                                     zRadians.ValueOrFailure());
      }
    }
  }
}