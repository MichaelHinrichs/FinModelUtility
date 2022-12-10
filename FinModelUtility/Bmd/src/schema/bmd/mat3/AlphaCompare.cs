using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class AlphaCompare : IDeserializable {
    public GxAlphaCompareType Func0;
    public byte Reference0;
    public GxAlphaOp MergeFunc;
    public GxAlphaCompareType Func1;
    public byte Reference1;
    public readonly byte padding1_ = 0xff;
    public readonly byte padding2_ = 0xff;
    public readonly byte padding3_ = 0xff;
  }
}