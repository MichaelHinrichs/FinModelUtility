using schema.binary;
using schema.binary.attributes;

namespace cmb.schema.cmb.qtrs {
  [BinarySchema]
  public partial class Qtrs : IBinaryConvertible {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
