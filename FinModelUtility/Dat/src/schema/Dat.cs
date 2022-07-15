using fin.util.asserts;

using schema;


namespace dat.schema {
  public class Dat : IDeserializable {
    public void Read(EndianBinaryReader er) {
      var fileHeaderLength = 0x20;
      var fileHeader = er.ReadNew<FileHeader>();
      Asserts.Equal(fileHeaderLength, er.Position);

      // Reads pointer table
      er.Position = fileHeaderLength + fileHeader.PointerTableOffset;
      er.ReadUInt32s((int) fileHeader.PointerCount);
    }
  }

  [Schema]
  public partial class FileHeader : IBiSerializable {
    public uint FileSize { get; set; }

    public uint PointerTableOffset { get; set; }
    public uint PointerCount { get; set; }
    public uint RootNodeCount { get; set; }
    public uint ReferenceNodeCount { get; set; }

    [StringLengthSource(4)]
    public string Unknown0 { get; set; }
    public uint Unknown1 { get; set; }
    public uint Unknown2 { get; set; }

  }
}