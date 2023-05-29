using System;
using System.Runtime.CompilerServices;

namespace fin.math {
  public static class FinTrig {
    // At this point, native C# approach is faster than FastMath.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float radians) => MathF.Cos(radians);

    // At this point, native C# approach is faster than FastMath.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float radians) => MathF.Sin(radians);

    // At this point, native C# approach is faster than FastMath.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(float value) => MathF.Acos(value);

    // At this point, native C# approach is faster than FastMath.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(float value) => MathF.Asin(value);

    // At this point, native C# approach is faster than FastMath.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2(float y, float x) => MathF.Atan2(y, x);
  }
}
