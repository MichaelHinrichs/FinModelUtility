using System;

namespace UoT {
  public enum BitSize {
    S_4B = 0,
    S_8B = 1,
    S_16B = 2,
    S_32B = 3,
  }

  public static class BitSizeUtil {
    public static BitSize Parse(byte bitSize) {
      switch (bitSize) {
        case 0:
          return BitSize.S_4B;
        case 1:
          return BitSize.S_8B;
        case 2:
          return BitSize.S_16B;
        case 3:
          return BitSize.S_32B;
        default:
          throw new NotSupportedException("Unsupported bit size.");
      }
    }
  }
}