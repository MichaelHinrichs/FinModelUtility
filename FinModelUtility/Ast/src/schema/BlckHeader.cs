using schema.binary;


namespace ast.schema {
  [BinarySchema]
  public partial class BlckHeader : IBiSerializable {
    private readonly string magic_ = "BLCK";

    public uint BlockSizeInBytes { get; private set; }

    public uint Unknown1 { get; private set; }
    public uint Unknown2 { get; private set; }

    public byte[] Unknowns { get; } = new byte[0x10];
  }
}