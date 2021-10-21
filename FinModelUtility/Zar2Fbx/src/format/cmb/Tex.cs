using System.IO;

using fin.io;
using fin.util.strings;

namespace zar.format.cmb {
  public class Tex : IDeserializable {
    public uint chunkSize { get; private set; }
    public Texture[] textures { get; private set; }

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("tex" + AsciiUtil.GetChar(0x20));

      this.chunkSize = r.ReadUInt32();

      this.textures = new Texture[r.ReadUInt32()];
      for (var i = 0; i < this.textures.Length; ++i) {
        var texture = new Texture();
        texture.Read(r);
        this.textures[i] = texture;
      }
    }
  }
}
