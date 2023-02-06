using cmb.schema.cmb;
using fin.util.strings;
using schema.binary;
using schema.binary.attributes.child_of;
using schema.binary.attributes.endianness;
using schema.binary.attributes.ignore;
using schema.binary.attributes.memory;
using schema.binary.attributes.size;
using System.IO;


namespace cmb.schema.ctxb {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public partial class Ctxb : IBinaryConvertible {
    public CtxbHeader Header { get; } = new();
    public CtxbTexChunk Chunk { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbHeader : IChildOf<Ctxb>, IBinaryConvertible {
    public Ctxb Parent { get; set; }

    private readonly string magic_ = "ctxb";

    [SizeOfStreamInBytes]
    public int CtxbSize { get; private set; }

    private readonly uint texCount_ = 1;
    private readonly uint padding_ = 0;

    [PointerTo($"{nameof(Parent)}.{nameof(Ctxb.Chunk)}")]
    public int ChunkOffset { get; private set; }

    [PointerTo($"{nameof(Parent)}.{nameof(Ctxb.Chunk)}.{nameof(CtxbTexChunk.Entry)}.{nameof(CtxbTexEntry.Data)}")]
    public int DataOffset { get; private set; }
  }

  [BinarySchema]
  public partial class CtxbTexChunk : IBinaryConvertible {
    private readonly string magic_ = "tex" + AsciiUtil.GetChar(0x20);
    private readonly int chunkSize_ = 0x30;

    private readonly uint texCount_ = 1;

    public CtxbTexEntry Entry { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbTexEntry : IBinaryConvertible {
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