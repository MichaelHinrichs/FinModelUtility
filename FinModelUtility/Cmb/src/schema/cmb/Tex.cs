using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Tex : IBiSerializable {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public Texture[] textures { get; private set; }
  }
}
