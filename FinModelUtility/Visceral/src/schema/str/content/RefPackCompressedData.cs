using schema.binary;

namespace visceral.schema.str.content {
  [BinarySchema]
  public partial class RefPackCompressedData : IContent {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public byte[] RawBytes { get; set; }
  }
}