using System;
using System.Runtime.CompilerServices;

using fin.math.interpolation;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public class SeparatePositionAxesTrack3dImpl
        : BScalarAxesTrack<Position, float, FloatInterpolator>,
          ISeparatePositionAxesTrack3d {
      private readonly IBone bone_;

      public SeparatePositionAxesTrack3dImpl(IBone bone,
                                 ReadOnlySpan<int> initialCapacityPerAxis) :
          base(3, initialCapacityPerAxis, new FloatInterpolator()) {
        this.bone_ = bone;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool TryGetInterpolatedFrame(float frame,
                                          out Position interpolatedValue,
                                          bool useLoopingInterpolation =
                                              false) {
        interpolatedValue = GetInterpolatedFrame(frame, useLoopingInterpolation);
        return true;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override Position GetInterpolatedFrame(
          float frame,
          bool useLoopingInterpolation = false) {
        var localPosition = this.bone_.LocalPosition;

        if (!this.axisTracks[0].TryGetInterpolatedFrame(frame, out var x)) {
          x = localPosition.X;
        }

        if (!this.axisTracks[1].TryGetInterpolatedFrame(frame, out var y)) {
          y = localPosition.Y;
        }

        if (!this.axisTracks[2].TryGetInterpolatedFrame(frame, out var z)) {
          z = localPosition.Z;
        }

        return new(x, y, z);
      }
    }
  }
}