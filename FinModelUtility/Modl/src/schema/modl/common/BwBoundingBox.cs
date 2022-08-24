using schema;


namespace modl.schema.modl.common {
  [BinarySchema]
  public partial class BwBoundingBox : IBiSerializable {
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float Z1 { get; set; }

    public float X2 { get; set; }
    public float Y2 { get; set; }
    public float Z2 { get; set; }
  }
}
