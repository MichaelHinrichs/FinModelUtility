using fin.color;

using schema;
using schema.attributes.ignore;


namespace fin.schema.color {
  [BinarySchema]
  public partial class Rgba4f : IColor, IBiSerializable {
    public float Rf { get; set; }
    public float Gf { get; set; }
    public float Bf { get; set; }
    public float Af { get; set; }

    [Ignore]
    public byte Rb => (byte) (this.Rf * 255);

    [Ignore]
    public byte Gb => (byte) (this.Gf * 255);

    [Ignore]
    public byte Bb => (byte) (this.Bf * 255);

    [Ignore]
    public byte Ab => (byte) (this.Af * 255);
  }
}