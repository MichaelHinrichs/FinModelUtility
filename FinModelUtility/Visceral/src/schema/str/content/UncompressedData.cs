using schema.binary;
using schema.binary.attributes.sequence;

namespace visceral.schema.str.content {
  [BinarySchema]
  public partial class UncompressedData : IContent {
    [RSequenceUntilEndOfStream]
    public byte[] Bytes { get; set; }
  }
}