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

      public void Set(IAxesTrack<float, Quaternion> other) {
        for (var i = 0; i < 3; ++i) {
          this.axisTracks_[i].Set(other.AxisTracks[i]);
        }
      }

      public void Set(int frame, int axis, float radians)
        => this.axisTracks_[axis].Set(frame, radians);

      public void Set(int frame, int axis, float radians, float tangent)
        => this.axisTracks_[axis].Set(frame, radians, tangent);

      public void Set(
          int frame,
          int axis,
          float radians,
          Optional<float> tangent)
        => this.axisTracks_[axis].Set(frame, radians, tangent);

      public Optional<Keyframe<float>>[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();

      public IRotation GetAlmostKeyframe(float frame)
        => new RotationImpl().SetRadians(
            this.axisTracks_[0]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .Assert(),
            this.axisTracks_[1]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .Assert(),
            this.axisTracks_[2]
                .GetInterpolatedFrame(frame, this.defaultRotation_)
                .Assert());

      public Quaternion GetInterpolatedFrame(float frame) {
        var xTrack = this.axisTracks_[0];
        var yTrack = this.axisTracks_[1];
        var zTrack = this.axisTracks_[2];

        var keyframe = (int) frame;

        // TODO: Properly interpolate between first and final keyframe
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

        var xRadians =
            xTrack.GetInterpolatedFrame(frame, this.defaultRotation_).Assert();
        var yRadians =
            yTrack.GetInterpolatedFrame(frame, this.defaultRotation_).Assert();
        var zRadians =
            zTrack.GetInterpolatedFrame(frame, this.defaultRotation_).Assert();

        return QuaternionUtil.Create(xRadians, yRadians, zRadians);
      }
    }
  }
}