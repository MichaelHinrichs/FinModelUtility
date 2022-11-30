using System.IO;

using schema;

namespace mod.schema {
  [BinarySchema]
  public partial class VtxMatrix : IBiSerializable {
    public short index = 0;
  }
}
