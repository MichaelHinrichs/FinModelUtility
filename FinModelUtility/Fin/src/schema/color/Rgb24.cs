using schema;


namespace fin.schema.color {
  [Schema]
  public partial class Rgb24 : IBiSerializable {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
  }
}
