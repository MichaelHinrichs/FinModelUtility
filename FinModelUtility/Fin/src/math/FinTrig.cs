using FastMath;
using System;

namespace fin.math {
  public static class FinTrig {
    private static readonly MemoizedCos MEMOIZED_COS = MemoizedCos.ConstructByMaxError(0.001f);
    private static readonly MemoizedSin MEMOIZED_SIN = MemoizedSin.ConstructByMaxError(0.001f);
    private static readonly MemoizedAcos MEMOIZED_ACOS = MemoizedAcos.ConstructByMaxError(0.001f);
    private static readonly MemoizedAsin MEMOIZED_ASIN = MemoizedAsin.ConstructByMaxError(0.001f);
    private static readonly MemoizedAtan2 MEMOIZED_ATAN2 = MemoizedAtan2.ConstructByMaxError(0.001f);

    public static Func<float, float> Cos => MEMOIZED_COS.CalculateUnbound;
    public static Func<float, float> Sin => MEMOIZED_SIN.CalculateUnbound;

    public static Func<float, float> Acos => MEMOIZED_ACOS.Calculate;
    public static Func<float, float> Asin => MEMOIZED_ASIN.Calculate;
    public static Func<float, float, float> Atan2 => MEMOIZED_ATAN2.Calculate;
  }
}
