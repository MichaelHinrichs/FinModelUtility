using schema.binary;
using schema.binary.attributes.sequence;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Qtrs : IBinaryConvertible {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
