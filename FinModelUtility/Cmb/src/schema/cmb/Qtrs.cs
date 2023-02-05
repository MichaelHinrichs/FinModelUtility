using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Qtrs : IBiSerializable {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
