using System;
using System.IO;

using fin.util.strings;

using schema;

using zar.schema.cmb;


namespace zar.schema.ctxb {
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

  [Schema]
  public partial class CtxbHeader : IBiSerializable {
    private readonly string magic_ = "ctxb";
    public int ChunkSize { get; private set; }
    private readonly uint texCount_ = 1;
    private readonly uint padding_ = 0;
    public int ChunkOffset { get; private set; }
    public int DataOffset { get; private set; }
  }

  [Schema]
  public partial class CtxbTexChunk : IBiSerializable {
    private readonly string magic_ = "tex" + AsciiUtil.GetChar(0x20);
    public int ChunkSize { get; private set; }
    private readonly uint texCount_ = 1;
    public CtxbTexEntry Entry { get; } = new();
  }

  public class CtxbTexEntry : IBiSerializable {
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

    public void Write(EndianBinaryWriter w)
      => throw new NotImplementedException();
  }
}