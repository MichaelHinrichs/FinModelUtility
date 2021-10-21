using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class TexMapper : IDeserializable {
    public short textureId;
    public TextureMinFilter minFilter;
    public TextureMagFilter magFilter;
    public TextureWrapMode wrapS;
    public TextureWrapMode wrapT;
    public float minLodBias;
    public float lodBias;
    public readonly byte[] borderColor = new byte[4];

    public void Read(EndianBinaryReader r) {
      // # Not an int because "-1" is 0xFFFF0000 and not 0xFFFFFFFF
      this.textureId = r.ReadInt16();
      r.ReadUInt16(); // Alignment
      this.minFilter = (TextureMinFilter) r.ReadUInt16();
      this.magFilter = (TextureMagFilter) r.ReadUInt16();
      this.wrapS = (TextureWrapMode) r.ReadUInt16();
      this.wrapT = (TextureWrapMode) r.ReadUInt16();
      this.minLodBias = r.ReadSingle();
      this.lodBias = r.ReadSingle();
      r.ReadBytes(this.borderColor);
    }
  }
}