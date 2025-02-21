﻿using System.Numerics;
using System.Runtime.CompilerServices;


namespace fin.math {
  public static class FinMath {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNumber Sqr<TNumber>(this TNumber value)
        where TNumber : INumber<TNumber>
      => value * value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange<TNumber>(this TNumber value,
                                          TNumber min,
                                          TNumber max)
        where TNumber : INumber<TNumber>
      => min <= value && value <= max;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInArea<TNumber>(this TNumber valueX,
                                         TNumber valueY,
                                         TNumber minX,
                                         TNumber minY,
                                         TNumber maxX,
                                         TNumber maxY)
        where TNumber : INumber<TNumber>
      => IsInRange(valueX, minX, maxX) && IsInRange(valueY, minY, maxY);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNumber MapRange<TNumber>(this TNumber value,
                                            TNumber minFrom,
                                            TNumber maxFrom,
                                            TNumber minTo,
                                            TNumber maxTo)
        where TNumber : INumber<TNumber>
      => minTo + (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNumber Clamp<TNumber>(this TNumber value,
                                         TNumber min,
                                         TNumber max)
        where TNumber : INumber<TNumber>
      => TNumber.Max(min, TNumber.Min(value, max));
  }
}