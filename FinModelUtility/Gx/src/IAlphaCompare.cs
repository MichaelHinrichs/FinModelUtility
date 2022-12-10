namespace gx {
  public enum GxAlphaCompareType : byte {
    Never = 0,
    Less = 1,
    Equal = 2,
    LEqual = 3,
    Greater = 4,
    NEqual = 5,
    GEqual = 6,
    Always = 7
  }

  public enum GxAlphaOp : byte {
    And = 0,
    Or = 1,
    XOR = 2,
    XNOR = 3
  }

  public interface IAlphaCompare {
    GxAlphaCompareType Func0 { get; }
    float Reference0 { get; }

    GxAlphaCompareType Func1 { get; }
    float Reference1 { get; }

    GxAlphaOp MergeFunc { get; }
  }
}
