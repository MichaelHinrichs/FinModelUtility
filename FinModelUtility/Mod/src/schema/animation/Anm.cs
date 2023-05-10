using schema.binary;
using schema.binary.attributes.sequence;


namespace mod.schema.animation {
  [BinarySchema]
  public partial class Anm : IBinaryConvertible {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public DcxWrapper[] Wrappers { get; set; }
  }
}