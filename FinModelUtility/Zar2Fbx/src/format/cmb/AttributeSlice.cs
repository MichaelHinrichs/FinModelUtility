using System.IO;

using schema;

namespace zar.format.cmb {
  public class AttributeSlice : IDeserializable {
    public uint size;
    public uint startOffset;

    public void Read(EndianBinaryReader r) {
      this.size = r.ReadUInt32();
      this.startOffset = r.ReadUInt32();
    }
  }
}
