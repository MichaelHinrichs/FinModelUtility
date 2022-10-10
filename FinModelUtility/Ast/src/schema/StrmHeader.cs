using schema;


namespace ast.schema {
  [BinarySchema]
  public partial class StrmHeader : IBiSerializable {
    private readonly string magic_ = "STRM";

    public uint MaybeSizeOfBlockSection { get; private set; }
    public uint Unk2 { get; private set; }

    public ushort ChannelCount { get; private set; }

    [IntegerFormat(SchemaIntegerType.UINT16)]
    public bool IsLooping { get; private set; }

    // 0x10
    public uint SampleRate { get; private set; }
    public uint SampleCount { get; private set; }

    public uint LoopStart { get; private set; }
    public uint LoopEnd { get; private set; }

    // 0x20
    public byte[] Unknowns { get; } = new byte[0x20];
  }
}