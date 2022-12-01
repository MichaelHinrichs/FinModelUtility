using cmb.schema.cmb;
using fin.util.strings;
using schema;
using schema.attributes.endianness;
using schema.attributes.ignore;
using schema.attributes.offset;
using System.IO;


namespace cmb.schema.ctxb {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public partial class Ctxb : IBiSerializable {
    public CtxbHeader Header { get; } = new();

    [Ignore]
    private uint BaseDataOffset => (uint)this.Header.DataOffset;

    public CtxbTexChunk Chunk { get; } = new();

    [Ignore]
    private uint ThisDataOffset => this.Chunk.Entry.dataOffset;

    [Ignore]
    private uint ThisDataLength => this.Chunk.Entry.dataLength;


    [Offset(nameof(BaseDataOffset), nameof(ThisDataOffset))]
    [ArrayLengthSource(nameof(ThisDataLength))]
    public byte[] Data { get; private set; }
  }

  [BinarySchema]
  public partial class CtxbHeader : IBiSerializable {
    private readonly string magic_ = "ctxb";
    public int ChunkSize { get; private set; }
    private readonly uint texCount_ = 1;
    private readonly uint padding_ = 0;
    public int ChunkOffset { get; private set; }
    public int DataOffset { get; private set; }
  }

  [BinarySchema]
  public partial class CtxbTexChunk : IBiSerializable {
    private readonly string magic_ = "tex" + AsciiUtil.GetChar(0x20);
    public int ChunkSize { get; private set; }
    private readonly uint texCount_ = 1;
    public CtxbTexEntry Entry { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbTexEntry : IBiSerializable {
    public uint dataLength { get; private set; }
    public ushort mimapCount { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isEtc1 { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isCubemap { get; private set; }

    public ushort width { get; private set; }
    public ushort height { get; private set; }
    public GlTextureFormat imageFormat { get; private set; }
    public uint dataOffset { get; private set; }

    [StringLengthSource(16)]
    public string name { get; private set; }
  }
}