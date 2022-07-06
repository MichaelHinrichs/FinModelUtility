using fin.data;

using schema;


namespace modl.protos {
  public class BwArchive : IDeserializable {
    public Rxet Rxet { get; } = new();
    public Dnos Dnos { get; } = new();

    public ListDictionary<string, BwFile> Files { get; } = new();

    public void Read(EndianBinaryReader er) {
      this.Rxet.Read(er);
      this.Dnos.Read(er);

      this.Files.Clear();

      while (er.Position < er.BaseStream.Length) {
        var bwFile = er.ReadNew<BwFile>();
        this.Files.Add(bwFile.Type, bwFile);
      }
    }
  }

  public class Rxet : IBiSerializable {
    public string FileName { get; private set; }

    public void Read(EndianBinaryReader er) {
      er.AssertMagicText("RXET");

      var dataLength = er.ReadUInt32();
      var dataOffset = er.Position;

      this.FileName = er.ReadString(er.ReadInt32());

      // TODO: What is the rest of this data?

      er.Position = dataOffset + dataLength;
    }

    public void Write(EndianBinaryWriter ew) =>
        throw new NotImplementedException();
  }

  [Schema]
  public partial class Dnos : IBiSerializable {
    private readonly string magic_ = "DNOS";

    public uint Length { get; private set; }

    [ArrayLengthSource(nameof(Length))] public byte[] Data { get; private set; }
  }

  public class Ldom : IDeserializable {
    public string FileName { get; private set; }

    public void Read(EndianBinaryReader er) {
      er.AssertMagicText("LDOM");

      var dataLength = er.ReadUInt32();
      var dataOffset = er.Position;

      this.FileName = er.ReadString(er.ReadInt32());

      // TODO: What is the rest of this data?

      er.Position = dataOffset + dataLength;
    }

    public void Write(EndianBinaryWriter ew) =>
        throw new NotImplementedException();
  }

  public class BwFile : IDeserializable {
    public string Type { get; private set; }
    public string FileName { get; private set; }
    public byte[] Data { get; private set; }

    public void Read(EndianBinaryReader er) {
      this.Type = er.ReadString(4);

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