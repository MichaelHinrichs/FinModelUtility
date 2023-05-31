using fin.data;
using fin.math.interpolation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    private class ScalarAxesTrack<TAxes, TAxis> :
          ScalarAxesTrack<TAxes, TAxis, TAxes> {
      public ScalarAxesTrack(
          int axisCount,
          ReadOnlySpan<int> initialKeyframeCapacitiesPerAxis,
          TAxis defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          GetInterpolatedFromAxesTrack getInterpolatedFromAxesTrack)
          : base(axisCount,
                 initialKeyframeCapacitiesPerAxis,
                 defaultValue,
                 axisInterpolator,
                 axisInterpolatorWithTangent,
                 getInterpolatedFromAxesTrack) { }
    }

    private class ScalarAxesTrack<TAxes, TAxis, TInterpolated> :
        IAxesTrack<TAxis, TInterpolated> {
      private readonly TAxis defaultValue_;

      public delegate TInterpolated GetInterpolatedFromAxesTrack(
          IAxesTrack<TAxis, TInterpolated> axesTrack,
          float frame,
          TAxis[] defaultValue);

      private TrackImpl<TAxis>[] axisTracks_;

      // TODO: Slow! Switch to a different approach here
      private readonly GetInterpolatedFromAxesTrack
          getInterpolatedFromAxesTrack_;

      public ScalarAxesTrack(
          int axisCount,
          ReadOnlySpan<int> initialKeyframeCapacitiesPerAxis,
          TAxis defaultValue,
          IInterpolator<TAxis> axisInterpolator,
          IInterpolatorWithTangents<TAxis> axisInterpolatorWithTangent,
          GetInterpolatedFromAxesTrack getInterpolatedFromAxesTrack) {
        this.axisTracks_ = new TrackImpl<TAxis>[axisCount];
        for (var i = 0; i < axisCount; ++i) {
          this.axisTracks_[i] =
              new TrackImpl<TAxis>(initialKeyframeCapacitiesPerAxis[i],
                                   axisInterpolator,
                                   axisInterpolatorWithTangent);
        }

        this.AxisTracks =
            new ReadOnlyCollection<ITrack<TAxis>>(this.axisTracks_);

        this.defaultValue_ = defaultValue;

        this.getInterpolatedFromAxesTrack_ = getInterpolatedFromAxesTrack;
      }

      public bool IsDefined => this.axisTracks_.Any(axis => axis.IsDefined);

      public int FrameCount {
        set {
          foreach (var axis in this.axisTracks_) {
            axis.FrameCount = value;
          }
        }
      }

      public void Set(IAxesTrack<TAxis, TInterpolated> other) {
        var otherAxisTracks = other.AxisTracks;
        for (var i = 0; i < otherAxisTracks.Count; ++i) {
          this.axisTracks_[i].Set(otherAxisTracks[i]);
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Set(
          int frame,
          int axis,
          TAxis value,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.axisTracks_[axis]
               .Set(frame,
                    value,
                    optionalIncomingTangent,
                    optionalOutgoingTangent);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Keyframe<ValueAndTangents<TAxis>>? GetKeyframe(int keyframe, int axis)
        => this.axisTracks_[axis].GetKeyframe(keyframe);


      public IReadOnlyList<ITrack<TAxis>> AxisTracks { get; }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Keyframe<ValueAndTangents<TAxis>>?[] GetAxisListAtKeyframe(int keyframe)
        => this.axisTracks_.Select(axis => axis.GetKeyframe(keyframe))
               .ToArray();


      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TInterpolated GetInterpolatedFrame(
          float frame,
          TAxis[] defaultValue,
          bool useLoopingInterpolation = false
      )
        => this.getInterpolatedFromAxesTrack_(this, frame, defaultValue);
    }
  }
}