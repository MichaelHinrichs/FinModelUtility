using fin.data;
using System.Collections.Generic;

using fin.math.interpolation;

namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrack3dImpl : IPositionTrack3d {
      private readonly ScalarAxesTrack<PositionStruct, float> impl_ =
          new(3,
              0,
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue) => new PositionStruct {
                X = axesTrack.AxisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
                Y = axesTrack.AxisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
                Z = axesTrack.AxisTracks[2].GetInterpolatedFrame(frame, defaultValue[2]),
              });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, PositionStruct> other)
        => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Keyframe<ValueAndTangents<float>>?[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public PositionStruct GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false)
        => this.impl_.GetInterpolatedFrame(
            frame,
            defaultValue,
            useLoopingInterpolation);
    }

    public class ScaleTrackImpl : IScale3dTrack {
      private readonly ScalarAxesTrack<ScaleStruct, float> impl_ =
          new(3,
              1,
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue)
                  => new ScaleStruct {
                    X = axesTrack.AxisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
                    Y = axesTrack.AxisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
                    Z = axesTrack.AxisTracks[2].GetInterpolatedFrame(frame, defaultValue[2]),
                  });

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, ScaleStruct> other) => this.impl_.Set(other);

      public void Set(
          int frame,
          int axis,
          float value,
          float? optionalIncomingTangent,
          float? optionalOutgoingTangent)
        => this.impl_.Set(frame,
                          axis,
                          value,
                          optionalIncomingTangent,
                          optionalOutgoingTangent);

      public Keyframe<ValueAndTangents<float>>?[] GetAxisListAtKeyframe(int keyframe)
        => this.impl_.GetAxisListAtKeyframe(keyframe);

      public ScaleStruct GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false
      )
        => this.impl_.GetInterpolatedFrame(frame, defaultValue,
                                           useLoopingInterpolation);
    }
  }
}