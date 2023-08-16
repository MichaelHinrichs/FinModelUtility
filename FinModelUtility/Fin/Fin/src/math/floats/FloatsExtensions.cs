using System;

namespace fin.math.floats {
  public static class FloatsExtensions {
    public const float ROUGHLY_EQUAL_ERROR = .0001f;

    public static bool IsRoughly(this float a, float b)
      => MathF.Abs(a - b) < ROUGHLY_EQUAL_ERROR;
  }
}