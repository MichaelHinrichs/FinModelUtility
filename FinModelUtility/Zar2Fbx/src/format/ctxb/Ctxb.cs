using System.IO;

using fin.util.strings;

using schema;

using zar.format.cmb;

namespace zar.format.ctxb {
  public class Ctxb : IDeserializable {
    public CtxbHeader Header { get; } = new();
    public CtxbTexChunk Chunk { get; } = new();
    public byte[] Data { get; private set; }

    public void Read(EndianBinaryReader r) {
      this.Header.Read(r);
      this.Chunk.Read(r);

      r.Position = this.Header.DataOffset + this.Chunk.Entry.dataOffset;
      this.Data = r.ReadBytes((int) this.Chunk.Entry.dataLength);
    }
  }

  public class CtxbHeader : IDeserializable {
    public int ChunkSize { get; private set; }
    public int ChunkOffset { get; private set; }
    public int DataOffset { get; private set; }

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("ctxb");

      this.ChunkSize = r.ReadInt32();
      r.AssertUInt32(1); // Tex count
      r.AssertUInt32(0);
      this.ChunkOffset = r.ReadInt32();
      this.DataOffset = r.ReadInt32();
    }
  }

  public class CtxbTexChunk : IDeserializable {
    public int ChunkSize { get; private set; }
    public CtxbTexEntry Entry { get; } = new();

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("tex" + AsciiUtil.GetChar(0x20));

      this.ChunkSize = r.ReadInt32();
      r.AssertUInt32(1); // Tex count
      this.Entry.Read(r);
    }
  }

  public class CtxbTexEntry : IDeserializable {
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