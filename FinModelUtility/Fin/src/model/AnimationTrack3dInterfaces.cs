using fin.model.impl;
using System.Numerics;


namespace fin.model {
  public interface IAxes3fTrack<TInterpolated> : IAxesTrack<float, TInterpolated> {
    void Set<TVector2>(int frame, float x, float y, float z) {
      Set(frame, 0, x);
      Set(frame, 1, y);
      Set(frame, 2, z);
    }

    void Set<TVector3>(int frame, TVector3 values) where TVector3 : IVector3 {
      Set(frame, 0, values.X);
      Set(frame, 1, values.Y);
      Set(frame, 2, values.Z);
    }

    void Set(int frame, Vector3 values) {
      Set(frame, 0, values.X);
      Set(frame, 1, values.Y);
      Set(frame, 2, values.Z);
    }
  }

  public interface IPositionTrack3d : IAxes3fTrack<PositionStruct> { }
  public interface IRadiansRotationTrack3d : IAxes3fTrack<Quaternion> { }
  public interface IScale3dTrack : IAxes3fTrack<ScaleStruct> { }
}
