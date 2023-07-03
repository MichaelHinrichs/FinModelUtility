using System.Numerics;

using fin.math;
using fin.math.interpolation;
using fin.util.asserts;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public class QuaternionAxesRotationTrack3dImpl
        : BScalarAxesTrack<Quaternion, float, FloatInterpolator>,
          IQuaternionAxesRotationTrack3d {
      private readonly IBone bone_;

      public QuaternionAxesRotationTrack3dImpl(IBone bone) : base(
          4,
          new[] { 0, 0, 0, 0 },
          new FloatInterpolator()) {
        this.bone_ = bone;
      }

      public override Quaternion GetInterpolatedFrame(float frame,
        bool useLoopingInterpolation = false) {
        Asserts.True(
            TryGetInterpolatedFrame(frame,
                                    out var value,
                                    useLoopingInterpolation));
        return value;
      }

      public bool TryGetInterpolatedFrame(float frame,
                                          out Quaternion interpolatedValue,
                                          bool useLoopingInterpolation =
                                              false) {
        // TODO: Might need to do something fancier here
        var defaultRotation = this.bone_.LocalRotation != null
            ? QuaternionUtil.Create(this.bone_.LocalRotation)
            : Quaternion.Identity;

        if (!this.axisTracks[0].TryGetInterpolatedFrame(frame, out var x, useLoopingInterpolation)) {
          x = defaultRotation.X;
        }

        if (!this.axisTracks[1].TryGetInterpolatedFrame(frame, out var y, useLoopingInterpolation)) {
          y = defaultRotation.Y;
        }

        if (!this.axisTracks[2].TryGetInterpolatedFrame(frame, out var z, useLoopingInterpolation)) {
          z = defaultRotation.Z;
        }

        if (!this.axisTracks[3].TryGetInterpolatedFrame(frame, out var w, useLoopingInterpolation)) {
          w = defaultRotation.W;
        }

        interpolatedValue = new(x, y, z, w);
        return true;
      }
    }
  }
}