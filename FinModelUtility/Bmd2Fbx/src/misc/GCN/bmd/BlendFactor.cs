using System.IO;

using bmd.formats;

using Tao.OpenGl;

namespace mkds.gcn.bmd {
  public enum SupportedGlBlendFactor {
    ZERO = 0,
    ONE = 1,
    SRC_COLOR = 768,
    ONE_MINUS_SRC_COLOR = 769,
    SRC_ALPHA = 770,
    ONE_MINUS_SRC_ALPHA = 771,
    DST_ALPHA = 772,
    ONE_MINUS_DST_ALPHA = 773,
  }

  public static class BlendFactor {
    public static SupportedGlBlendFactor BmdToGl(BmdBlendFactor bmdBlendFactor)
      => bmdBlendFactor switch
      {
          BmdBlendFactor.ZERO => SupportedGlBlendFactor.ZERO,
          BmdBlendFactor.ONE => SupportedGlBlendFactor.ONE,
          BmdBlendFactor.SRC_COLOR => SupportedGlBlendFactor.SRC_COLOR,
          BmdBlendFactor.ONE_MINUS_SRC_COLOR => SupportedGlBlendFactor.ONE_MINUS_SRC_COLOR,
          BmdBlendFactor.SRC_ALPHA => SupportedGlBlendFactor.SRC_ALPHA,
          BmdBlendFactor.ONE_MINUS_SRC_ALPHA => SupportedGlBlendFactor.ONE_MINUS_SRC_ALPHA,
          BmdBlendFactor.DST_ALPHA => SupportedGlBlendFactor.DST_ALPHA,
          BmdBlendFactor.ONE_MINUS_DST_ALPHA => SupportedGlBlendFactor.ONE_MINUS_DST_ALPHA,
          _ => throw new InvalidDataException(
                   $"Invalid bmdBlendFactor '{bmdBlendFactor}'"),
      };
  }
}