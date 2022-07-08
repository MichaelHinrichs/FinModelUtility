using System.IO;

using schema;

namespace mod.schema {
  [Schema]
  public partial class VtxMatrix : IBiSerializable {
    public short index = 0;
  }
}
