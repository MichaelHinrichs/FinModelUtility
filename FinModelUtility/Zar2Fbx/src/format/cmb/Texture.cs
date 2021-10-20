using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Texture : IDeserializable {
    public uint dataLength;
    public ushort mimapCount;
    public bool isEtc1;
    public bool isCubemap;
    public ushort width;
    public ushort height;
    public GlTextureFormat imageFormat;
    public uint dataOffset;
    public string name;

    public void Read(EndianBinaryReader r) {
      this.dataLength = r.ReadUInt32();
      this.mimapCount = r.ReadUInt16();
      this.isEtc1 = r.ReadByte() != 0;
      this.isCubemap = r.ReadByte() != 0;
      this.width = r.ReadUInt16();
      this.height = r.ReadUInt16();
      this.imageFormat = (GlTextureFormat) r.ReadUInt32();
      this.dataOffset = r.ReadUInt32();
      this.name = r.ReadString(16);
    }
  }
}
