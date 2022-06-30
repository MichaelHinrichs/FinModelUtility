using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class VtxMatrix : IBiSerializable {
    public short index = 0;
  }
}
