using System.IO;

using schema;

namespace mod.gcn {
  public interface IColour<T> : IBiSerializable {
    T R { get; set; }
    T G { get; set; }
    T B { get; set; }
    T A { get; set; }

    string? ToString() => $"{this.R} {this.G} {this.B} {this.A}";
  }

  [Schema]
  public partial class ColourU8 : IColour<byte> {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }

  [Schema]
  public partial class ColourU16 : IColour<ushort> {
    public ushort R { get; set; }
    public ushort G { get; set; }
    public ushort B { get; set; }
    public ushort A { get; set; }
  }
}