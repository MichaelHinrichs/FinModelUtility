using fin.color;

using schema;
using schema.attributes.ignore;


namespace fin.schema.color {
  [BinarySchema]
  public partial class Rgba32 : IColor, IBiSerializable {
    public byte Rb { get; set; }
    public byte Gb { get; set; }
    public byte Bb { get; set; }
    public byte Ab { get; set; }

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