using System;

namespace fin.math.floats {
  public static class FloatsExtensions {
    public static bool IsRoughly(this float a, float b)
      => MathF.Abs(a - b) < .0001f;
  }
}