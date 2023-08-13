using System;

using MathNet.Numerics;

namespace fin.math {
  public static class RadiansUtil {
    private const float PI = MathF.PI;
    private const float PI2 = 2 * PI;
    private const float PI3 = 3 * PI;

    public static float CalculateRadiansTowards(float from, float to) {
      var difference = to - from;
      if (Math.Abs(difference).AlmostEqual(PI, .001f)) {
        return difference;
      }

      return (((difference % PI2) + PI3) % PI2) - PI;
    }
  }
}