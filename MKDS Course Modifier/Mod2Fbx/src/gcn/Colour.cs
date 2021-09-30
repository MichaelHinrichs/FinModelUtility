using System.IO;

namespace mod.gcn {
  public interface IColour<T> : IGcnSerializable {
    T R { get; set; }
    T G { get; set; }
    T B { get; set; }
    T A { get; set; }

    string? ToString() => $"{this.R} {this.G} {this.B} {this.A}";
  }

  public class ColourU8 : IColour<byte> {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public void Read(EndianBinaryReader reader) {
      this.R = reader.ReadByte();
      this.G = reader.ReadByte();
      this.B = reader.ReadByte();
      this.A = reader.ReadByte();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.R);
      writer.Write(this.G);
      writer.Write(this.B);
      writer.Write(this.A);
    }
  }

  public class ColourU16 : IColour<ushort> {
    public ushort R { get; set; }
    public ushort G { get; set; }
    public ushort B { get; set; }
    public ushort A { get; set; }

    public void Read(EndianBinaryReader reader) {
      this.R = reader.ReadUInt16();
      this.G = reader.ReadUInt16();
      this.B = reader.ReadUInt16();
      this.A = reader.ReadUInt16();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.R);
      writer.Write(this.G);
      writer.Write(this.B);
      writer.Write(this.A);
    }
  }
}