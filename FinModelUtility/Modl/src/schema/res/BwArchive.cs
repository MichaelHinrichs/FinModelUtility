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
    [EndianOrdered] private readonly string magic_ = "SOND";
    [ArrayLengthSource(SchemaIntType.UINT32)] public byte[] Data { get; private set; }
  }

  public class BwFile : IDeserializable {
    public string Type { get; private set; }
    public string FileName { get; private set; }
    public byte[] Data { get; private set; }

    public void Read(EndianBinaryReader er) {
      this.Type = new string(er.ReadChars(4).Reverse().ToArray());

      var dataLength = er.ReadUInt32();
      var dataOffset = er.Position;

      this.FileName = er.ReadString(er.ReadInt32());

      er.Position = dataOffset;
      this.Data = er.ReadBytes((int) dataLength);

      er.Position = dataOffset + dataLength;
    }

    public void Write(EndianBinaryWriter ew) =>
        throw new NotImplementedException();
  }
}