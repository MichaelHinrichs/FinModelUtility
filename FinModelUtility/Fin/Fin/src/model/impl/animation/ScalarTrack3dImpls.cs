using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using fin.data;
using fin.math.interpolation;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrack3dImpl
        : BScalarAxesTrack<Position, float, FloatInterpolator>,
          IPositionTrack3d {
      public PositionTrack3dImpl(ReadOnlySpan<int> initialCapacityPerAxis) :
          base(3, initialCapacityPerAxis, new FloatInterpolator()) { }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override Position GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false) {
        if (!this.axisTracks[0].TryGetInterpolatedFrame(frame, out var x)) {
          x = defaultValue[0];
        }
        if (!this.axisTracks[1].TryGetInterpolatedFrame(frame, out var y)) {
          y = defaultValue[1];
        }
        if (!this.axisTracks[2].TryGetInterpolatedFrame(frame, out var z)) {
          z = defaultValue[2];
        }

        return new(x, y, z);
      }
    }

    public class ScaleTrackImpl
        : BScalarAxesTrack<Scale, float, FloatInterpolator>,
          IScale3dTrack {
      public ScaleTrackImpl(ReadOnlySpan<int> initialCapacityPerAxis) :
          base(3, initialCapacityPerAxis, new FloatInterpolator()) { }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override Scale GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false) {
        if (!this.axisTracks[0].TryGetInterpolatedFrame(frame, out var x)) {
          x = defaultValue[0];
        }
        if (!this.axisTracks[1].TryGetInterpolatedFrame(frame, out var y)) {
          y = defaultValue[1];
        }
        if (!this.axisTracks[2].TryGetInterpolatedFrame(frame, out var z)) {
          z = defaultValue[2];
        }

        return new(x, y, z);
      }
    }
  }
}