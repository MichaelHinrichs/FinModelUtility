using schema.binary;

namespace geo.schema.str.content {
  [BinarySchema]
  public partial class CompressedData : IContent {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public byte[] RawBytes { get; set; }
  }
}