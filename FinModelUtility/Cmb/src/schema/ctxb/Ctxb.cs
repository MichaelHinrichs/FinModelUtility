using cmb.schema.cmb;
using fin.util.strings;
using schema;
using schema.attributes.child_of;
using schema.attributes.endianness;
using schema.attributes.ignore;
using schema.attributes.memory;
using schema.attributes.size;
using System.IO;


namespace cmb.schema.ctxb {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public partial class Ctxb : IBiSerializable {
    public CtxbHeader Header { get; } = new();
    public CtxbTexChunk Chunk { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbHeader : IChildOf<Ctxb>, IBiSerializable {
    public Ctxb Parent { get; set; }

    private readonly string magic_ = "ctxb";

    [SizeOfStreamInBytes]
    public int CtxbSize { get; private set; }

    private readonly uint texCount_ = 1;
    private readonly uint padding_ = 0;

    [PointerTo(nameof(Parent.Chunk))]
    public int ChunkOffset { get; private set; }

    [PointerTo(nameof(Parent.Chunk.Entry.Data))]
    public int DataOffset { get; private set; }
  }

  [BinarySchema]
  public partial class CtxbTexChunk : IBiSerializable {
    private readonly string magic_ = "tex" + AsciiUtil.GetChar(0x20);
    private readonly int chunkSize_ = 0x30;

    private readonly uint texCount_ = 1;

    public CtxbTexEntry Entry { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbTexEntry : IBiSerializable {
    public uint DataLength { get; private set; }
    public ushort mimapCount { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isEtc1 { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isCubemap { get; private set; }

    public ushort width { get; private set; }
    public ushort height { get; private set; }
    public GlTextureFormat imageFormat { get; private set; }

    [StringLengthSource(16)]
    public string name { get; private set; }

    private uint padding_;

    [Ignore]
    private bool includeExtraPadding_ 
      => CmbHeader.Version >= CmbVersion.LUIGIS_MANSION_3D;

    [IfBoolean(nameof(includeExtraPadding_))]
    [ArrayLengthSource(56)]
    private byte[]? extraPadding_;

    [ArrayLengthSource(nameof(DataLength))]
    public byte[] Data { get; private set; }
  }
}