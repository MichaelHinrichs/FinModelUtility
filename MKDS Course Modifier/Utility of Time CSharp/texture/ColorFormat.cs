using System;

namespace UoT {
  public enum ColorFormat {
    RGBA = 0,
    YUV = 1,
    /// <summary>
    ///   Color-indexed
    /// </summary>
    CI = 2,
    /// <summary>
    ///   Intensity/alpha
    /// </summary>
    IA = 3,
    /// <summary>
    ///   Intensity
    /// </summary>
    I = 4,
  }

  public static class ColorFormatUtil {
    public static ColorFormat Parse(byte colorFormat) {
      switch (colorFormat) {
        case 0:
          return ColorFormat.RGBA;
        case 2:
          return ColorFormat.CI;
        case 3:
          return ColorFormat.IA;
        case 4:
          return ColorFormat.I;
        case 1:
        default:
          throw new NotSupportedException("Unsupported color format.");
      }
    }
  }
}
