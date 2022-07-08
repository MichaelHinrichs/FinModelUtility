using System.IO;

using bmd.schema.bmd.mat3;


namespace mkds.gcn.bmd {
  public enum SupportedGlBlendMode {
    NONE = 0,
    ADD = 32774,
    REVERSE_SUBTRACT = 32779,
    SUBTRACT = 32778,
  }

  public static class Blending {
    public static SupportedGlBlendMode BmdToGl(BmdBlendMode bmdBlendMode)
      => bmdBlendMode switch {
          BmdBlendMode.NONE => SupportedGlBlendMode.NONE,
          BmdBlendMode.ADD  => SupportedGlBlendMode.ADD,
          BmdBlendMode.REVERSE_SUBTRACT =>
          SupportedGlBlendMode.REVERSE_SUBTRACT,
          BmdBlendMode.SUBTRACT => SupportedGlBlendMode.SUBTRACT,
          _ => throw new InvalidDataException(
                   $"Invalid bmdBlendMode '{bmdBlendMode}'"),
      };
  }
}