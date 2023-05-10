using schema.binary;
using schema.binary.attributes.sequence;

namespace visceral.schema.str.content {
  [BinarySchema]
  public partial class RefPackCompressedData : IContent {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public byte[] RawBytes { get; set; }
  }
}