using fin.data;

using modl.schema.res.texr;

using schema;


namespace modl.schema.res {
  public class BwArchive : IDeserializable {
    public Texr Texr { get; } = new();
    public Sond Sond { get; } = new();

    public ListDictionary<string, BwFile> Files { get; } = new();

    public void Read(EndianBinaryReader er) {
      this.Texr.Read(er);
      this.Sond.Read(er);

      this.Files.Clear();

      while (!er.Eof) {
        var bwFile = er.ReadNew<BwFile>();
        this.Files.Add(bwFile.Type, bwFile);
      }
    }
  }

  [BinarySchema]
  public partial class Sond : IBiSerializable {
    private readonly string magic_ = "DNOS"; // SOND backwards

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public byte[] Data { get; private set; }
  }

  public class BwFile : IDeserializable {
    public string Type { get; private set; }
    public string FileName { get; private set; }
    public byte[] Data { get; private set; }

    public void Read(EndianBinaryReader er) {
      SectionHeaderUtil.ReadNameAndSize(
          er,
          out var sectionName,
          out var dataLength);
      this.Type = sectionName;
      var dataOffset = er.Position;

      this.FileName = er.ReadString(er.ReadInt32());

      er.Position = dataOffset;
      this.Data = er.ReadBytes((int) dataLength);

      er.Position = dataOffset + dataLength;
    }

    public void Write(ISubEndianBinaryWriter ew) =>
        throw new NotImplementedException();
  }
}