using System.Numerics;

namespace fin.math {
  public static class FinMath {
    public static bool IsInRange<TNumber>(TNumber value,
                                          TNumber min,
                                          TNumber max)
        where TNumber : INumber<TNumber>
      => min <= value && value <= max;

    public static bool IsInArea<TNumber>(TNumber valueX,
                                         TNumber valueY,
                                         TNumber minX,
                                         TNumber minY,
                                         TNumber maxX,
                                         TNumber maxY)
        where TNumber : INumber<TNumber>
      => IsInRange(valueX, minX, maxX) && IsInRange(valueY, minY, maxY);

    public static TNumber MapRange<TNumber>(TNumber value,
                                            TNumber minFrom,
                                            TNumber maxFrom,
                                            TNumber minTo,
                                            TNumber maxTo)
        where TNumber : INumber<TNumber>
      => minTo + (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo);

    public static TNumber Clamp<TNumber>(TNumber value,
                                         TNumber min,
                                         TNumber max)
        where TNumber : INumber<TNumber>
      => TNumber.MaxMagnitude(min, TNumber.MinMagnitude(value, max));
  }
}