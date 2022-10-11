using System.Numerics;


namespace fin.math {
  public static class FinMath {
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