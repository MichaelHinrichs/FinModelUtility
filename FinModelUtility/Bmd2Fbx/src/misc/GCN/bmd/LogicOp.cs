using System.IO;

using bmd.schema.bmd.mat3;


namespace mkds.gcn.bmd {
  public enum SupportedGlLogicOp {
    CLEAR = 5376,
    AND = 5377,
    AND_REVERSE = 5378,
    COPY = 5379,
    AND_INVERTED = 5380,
    NOOP = 5381,
    XOR = 5382,
    OR = 5383,
    NOR = 5384,
    EQUIV = 5385,
    INVERT = 5386,
    OR_REVERSE = 5387,
    COPY_INVERTED = 5388,
    OR_INVERTED = 5389,
    NAND = 5390,
    SET = 5391,
  }

  public static class LogicOp {
    public static SupportedGlLogicOp BmdToGl(BmdLogicOp bmdLogicOp)
      => bmdLogicOp switch
      {
          BmdLogicOp.CLEAR => SupportedGlLogicOp.CLEAR,
          BmdLogicOp.AND => SupportedGlLogicOp.AND,
          BmdLogicOp.AND_REVERSE => SupportedGlLogicOp.AND_REVERSE,
          BmdLogicOp.COPY => SupportedGlLogicOp.COPY,
          BmdLogicOp.AND_INVERTED => SupportedGlLogicOp.AND_INVERTED,
          BmdLogicOp.NOOP => SupportedGlLogicOp.NOOP,
          BmdLogicOp.XOR => SupportedGlLogicOp.XOR,
          BmdLogicOp.OR => SupportedGlLogicOp.OR,
          BmdLogicOp.NOR => SupportedGlLogicOp.NOR,
          BmdLogicOp.EQUIV => SupportedGlLogicOp.EQUIV,
          BmdLogicOp.INVERT => SupportedGlLogicOp.INVERT,
          BmdLogicOp.OR_REVERSE => SupportedGlLogicOp.OR_REVERSE,
          BmdLogicOp.COPY_INVERTED => SupportedGlLogicOp.COPY_INVERTED,
          BmdLogicOp.OR_INVERTED => SupportedGlLogicOp.OR_INVERTED,
          BmdLogicOp.NAND => SupportedGlLogicOp.NAND,
          BmdLogicOp.SET => SupportedGlLogicOp.SET,
        _ => throw new InvalidDataException(
                   $"Invalid bmdLogicOp '{bmdLogicOp}'"),
      };
  }
}