namespace UoT {
  // TODO: Split these out into separate files.
  public static class FunctionsCs {
    /// <summary>
    ///   Calculates the nearest power of 2 under a given value. This is useful
    ///   for textures widths/heights, which need to be exact powers of 2.
    /// </summary>
    public static ulong Pow2(ulong val) {
      ulong i = 1;
      while (i < val) {
        i <<= 1;
      }
      return i;
    }

    /// <summary>
    ///   Calculates which n results in 2^n closest under a given value.
    /// </summary>
    public static ulong PowOf(ulong val) {
      ulong num = 1;
      ulong i = 0;
      while (num < val) {
        num <<= 1;
        i++;
      }
      return i;
    }
  }
}