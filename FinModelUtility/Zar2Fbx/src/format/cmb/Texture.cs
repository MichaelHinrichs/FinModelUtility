using System.IO;

using schema;

namespace zar.format.cmb {
  public class Texture : IDeserializable {
    public uint dataLength { get; private set; }
    public ushort mimapCount { get; private set; }
    public bool isEtc1 { get; private set; }
    public bool isCubemap { get; private set; }
    public ushort width { get; private set; }
    public ushort height { get; private set; }
    public GlTextureFormat imageFormat { get; private set; }
    public uint dataOffset { get; private set; }
    public string name { get; private set; }

    public void Read(EndianBinaryReader r) {
      this.dataLength = r.ReadUInt32();
      this.mimapCount = r.ReadUInt16();
      this.isEtc1 = r.ReadByte() != 0;
      this.isCubemap = r.ReadByte() != 0;
      this.width = r.ReadUInt16();
      this.height = r.ReadUInt16();
      this.imageFormat = (GlTextureFormat) r.ReadUInt32();
      this.dataOffset = r.ReadUInt32();
      this.name = CmbStringUtil.ReadString(r, 16);
    }
  }
}
