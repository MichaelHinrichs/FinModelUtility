using schema.binary;
using schema.binary.attributes;

namespace mod.schema.animation {
  [BinarySchema]
  public partial class Anm : IBinaryConvertible {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public DcxWrapper[] Wrappers { get; set; }
  }
}