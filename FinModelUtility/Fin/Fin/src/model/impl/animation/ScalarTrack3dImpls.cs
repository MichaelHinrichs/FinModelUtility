using System;
using System.Runtime.CompilerServices;

using fin.math.interpolation;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrack3dImpl
        : BScalarAxesTrack<Position, float, FloatInterpolator>,
          IPositionTrack3d {
      private readonly IBone bone_;

      public PositionTrack3dImpl(IBone bone,
                                 ReadOnlySpan<int> initialCapacityPerAxis) :
          base(3, initialCapacityPerAxis, new FloatInterpolator()) {
        this.bone_ = bone;
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

    public class ScaleTrackImpl
        : BScalarAxesTrack<Scale, float, FloatInterpolator>,
          IScale3dTrack {
      private readonly IBone bone_;

      public ScaleTrackImpl(IBone bone,
                            ReadOnlySpan<int> initialCapacityPerAxis) :
          base(3, initialCapacityPerAxis, new FloatInterpolator()) {
        this.bone_ = bone;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override Scale GetInterpolatedFrame(
          float frame,
          bool useLoopingInterpolation = false) {
        var localScale = this.bone_.LocalScale;

        if (!this.axisTracks[0].TryGetInterpolatedFrame(frame, out var x)) {
          x = localScale?.X ?? 1;
        }

        if (!this.axisTracks[1].TryGetInterpolatedFrame(frame, out var y)) {
          y = localScale?.Y ?? 1;
        }

        if (!this.axisTracks[2].TryGetInterpolatedFrame(frame, out var z)) {
          z = localScale?.Z ?? 1;
        }

        return new(x, y, z);
      }
    }
  }
}