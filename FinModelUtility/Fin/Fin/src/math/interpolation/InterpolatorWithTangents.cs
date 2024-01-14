using System;
using System.Runtime.CompilerServices;

namespace fin.math.interpolation {
  public static class InterpolatorWithTangents {
    /// <summary>
    ///   Shamelessly copied from:
    ///   https://answers.unity.com/questions/464782/t-is-the-math-behind-animationcurveevaluate.html
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InterpolateFloats(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var dt = toTime - fromTime;

      var m0 = fromTangent * dt;
      var m1 = toTangent * dt;

      var t1 = (time - fromTime) / (toTime - fromTime);
      var t2 = t1 * t1;
      var t3 = t2 * t1;

      var a = 2 * t3 - 3 * t2 + 1;
      var b = t3 - 2 * t2 + t1;
      var c = t3 - t2;
      var d = -2 * t3 + 3 * t2;

      var interpolated = a * fromValue + b * m0 + c * m1 + d * toValue;
      return interpolated;
    }
  }
}