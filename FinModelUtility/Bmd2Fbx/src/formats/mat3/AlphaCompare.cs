using schema;


namespace bmd.formats.mat3 {
  public enum GxCompareType : byte {
    Never = 0,
    Less = 1,
    Equal = 2,
    LEqual = 3,
    Greater = 4,
    NEqual = 5,
    GEqual = 6,
    Always = 7
  }

  public enum GXAlphaOp : byte {
    And = 0,
    Or = 1,
    XOR = 2,
    XNOR = 3
  }

  [Schema]
  public partial class AlphaCompare : IDeserializable {
    public GxCompareType Func0;
    public byte Reference0;
    public GXAlphaOp MergeFunc;
    public GxCompareType Func1;
    public byte Reference1;
    public readonly byte padding1_ = 0xff;
    public readonly byte padding2_ = 0xff;
    public readonly byte padding3_ = 0xff;
  }
}