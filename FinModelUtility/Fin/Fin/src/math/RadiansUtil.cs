using System;
using System.Runtime.CompilerServices;

using MathNet.Numerics;

namespace fin.math {
  public static class RadiansUtil {
    private const float PI = MathF.PI;
    private const float PI2 = 2 * PI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalculateRadiansTowards(float from, float to) {
      var difference = to - from;
      if (Math.Abs(difference).AlmostEqual(PI, .001f)) {
        return difference;
      }

      return (CustomMod_(difference + PI, PI2) - PI);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CustomMod_(float a, float n)
      => a - MathF.Floor(a / n) * n;
  }
}