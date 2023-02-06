using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Tex : IBinaryConvertible {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public Texture[] textures { get; private set; }
  }
}
