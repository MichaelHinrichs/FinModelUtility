using schema.binary;

namespace mod.schema {
  [BinarySchema]
  public partial class VtxMatrix : IBinaryConvertible {
    public short index = 0;
  }
}
