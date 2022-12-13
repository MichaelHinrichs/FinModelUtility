using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class DepthFunction : IDeserializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool Enable;

    public GxAlphaCompareType Func;

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public byte WriteNewValueIntoDepthBuffer;

    private readonly byte padding_ = 0xff;
  }
}