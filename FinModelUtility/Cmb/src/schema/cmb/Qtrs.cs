using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Qtrs : IBiSerializable {
    public readonly string magic = "qtrs";
    public uint chunkSize { get; private set; }

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
