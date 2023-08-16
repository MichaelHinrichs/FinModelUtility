using System.Runtime.CompilerServices;

using fin.model;

namespace fin.math.interpolation {
  public readonly struct PositionInterpolator : IInterpolator<Position> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Position Interpolate(
        Position p1,
        Position p2,
        float progress)
      => new(p1.X * (1 - progress) + p2.X * progress,
             p1.Y * (1 - progress) + p2.Y * progress,
             p1.Z * (1 - progress) + p2.Z * progress);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Position Interpolate(float fromTime,
                                Position p1,
                                float fromTangent,
                                float toTime,
                                Position p2,
                                float toTangent,
                                float time)
      => new(
          InterpolatorWithTangents.InterpolateFloats(
              fromTime,
              p1.X,
              fromTangent,
              toTime,
              p2.X,
              toTangent,
              time),
          InterpolatorWithTangents.InterpolateFloats(
              fromTime,
              p1.Y,
              fromTangent,
              toTime,
              p2.Y,
              toTangent,
              time),
          InterpolatorWithTangents.InterpolateFloats(
              fromTime,
              p1.Z,
              fromTangent,
              toTime,
              p2.Z,
              toTangent,
              time));
  }
}