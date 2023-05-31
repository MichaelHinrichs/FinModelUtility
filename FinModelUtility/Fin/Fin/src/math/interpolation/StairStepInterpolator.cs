using System.Runtime.CompilerServices;

namespace fin.math.interpolation {
  public readonly struct StairStepInterpolator<T> : IInterpolator<T> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Interpolate(T fromValue, T toValue, float progress)
      => fromValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Interpolate(float fromTime,
                         T fromValue,
                         float fromTangent,
                         float toTime,
                         T toValue,
                         float toTangent,
                         float time)
      => fromValue;
  }
}