using System.IO;

namespace mod.gcn {
  public class VtxMatrix : IGcnSerializable {
    public short index = 0;

    public void Read(EndianBinaryReader reader) {
      this.index = reader.ReadInt16();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.index);
    }
  }
}
