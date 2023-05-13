using System;

namespace fin.math {
  public static class RadiansUtil {
    public static float angleDifference(float lhs, float rhs) {
      var pi = MathF.PI;
      var pi2 = 2 * MathF.PI;
      var pi3 = 3 * MathF.PI;
      return ((((lhs - rhs) % pi2) + pi3) % pi2) - pi;
    }
  }
}
