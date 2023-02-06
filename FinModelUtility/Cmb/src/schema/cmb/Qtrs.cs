using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Qtrs : IBinaryConvertible {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
