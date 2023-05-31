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
          bool useLoopingInterpolation = false)
        => new(
            this.axisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
            this.axisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
            this.axisTracks[2].GetInterpolatedFrame(frame, defaultValue[2]));
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
          bool useLoopingInterpolation = false)
        => new(
            this.axisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
            this.axisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
            this.axisTracks[2].GetInterpolatedFrame(frame, defaultValue[2]));
    }
  }
}