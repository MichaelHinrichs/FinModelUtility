using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class AlphaCompare : IAlphaCompare, IDeserializable {
    public GxCompareType Func0 { get; set; }

    [NumberFormat(SchemaNumberType.UN8)]
    public float Reference0 { get; set; }

    public GxAlphaOp MergeFunc { get; set; }
    public GxCompareType Func1 { get; set; }

    [NumberFormat(SchemaNumberType.UN8)]
    public float Reference1 { get; set; }

    public readonly byte padding1_ = 0xff;
    public readonly byte padding2_ = 0xff;
    public readonly byte padding3_ = 0xff;
  }
}