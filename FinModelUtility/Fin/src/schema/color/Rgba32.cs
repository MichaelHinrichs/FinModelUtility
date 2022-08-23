using schema;


namespace fin.schema.color {
  [BinarySchema]
  public partial class Rgba32 : IBiSerializable {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }
}
