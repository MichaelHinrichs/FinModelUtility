using fin.color;

using schema.binary;
using schema.binary.attributes.ignore;


namespace fin.schema.color {
  [BinarySchema]
  public partial class Rgb24 : IColor, IBiSerializable {
    public byte Rb { get; set; }
    public byte Gb { get; set; }
    public byte Bb { get; set; }

    [Ignore]
    public byte Ab => 255;

    [Ignore]
    public float Rf => this.Rb / 255f;

    [Ignore]
    public float Gf => this.Gb / 255f;

    [Ignore]
    public float Bf => this.Bb / 255f;

    [Ignore]
    public float Af => this.Ab / 255f;
  }
}
