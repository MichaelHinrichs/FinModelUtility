using fin.data;
using System.Collections.Generic;

using fin.math.interpolation;

namespace fin.model.impl {
  public partial class ModelImpl {
    // TODO: Rethink this, this is all getting way too complicated.

    public class PositionTrack3dImpl : IPositionTrack3d {
      private readonly ScalarAxesTrack<Position, float> impl_ =
          new(3,
              0,
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue) => new Position(
                axesTrack.AxisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
                axesTrack.AxisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
                axesTrack.AxisTracks[2].GetInterpolatedFrame(frame, defaultValue[2])
              ));

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, Position> other)
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

      public Position GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false)
        => this.impl_.GetInterpolatedFrame(
            frame,
            defaultValue,
            useLoopingInterpolation);
    }

    public class ScaleTrackImpl : IScale3dTrack {
      private readonly ScalarAxesTrack<Scale, float> impl_ =
          new(3,
              1,
              Interpolator.Float,
              InterpolatorWithTangents.Float,
              (axesTrack, frame, defaultValue)
                  => new Scale(
                    axesTrack.AxisTracks[0].GetInterpolatedFrame(frame, defaultValue[0]),
                    axesTrack.AxisTracks[1].GetInterpolatedFrame(frame, defaultValue[1]),
                    axesTrack.AxisTracks[2].GetInterpolatedFrame(frame, defaultValue[2])
                  ));

      public IReadOnlyList<ITrack<float>> AxisTracks => this.impl_.AxisTracks;

      public bool IsDefined => this.impl_.IsDefined;

      public int FrameCount {
        set => this.impl_.FrameCount = value;
      }

      public void Set(IAxesTrack<float, Scale> other) => this.impl_.Set(other);

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

      public Scale GetInterpolatedFrame(
          float frame,
          float[] defaultValue,
          bool useLoopingInterpolation = false
      )
        => this.impl_.GetInterpolatedFrame(frame, defaultValue,
                                           useLoopingInterpolation);
    }
  }
}