using schema;

namespace cmb.schema.cmb {
  [Schema]
  public partial class Qtrs : IBiSerializable {
    public readonly string magic = "qtrs";
    public uint chunkSize { get; private set; }

    [ArrayLengthSource(SchemaIntType.UINT32)]
    public BoundingBox[] boundingBoxes { get; private set; }
  }
}
