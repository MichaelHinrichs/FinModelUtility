using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class VtxMatrix : IGcnSerializable {
    public short index = 0;

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.index);
    }
  }
}
