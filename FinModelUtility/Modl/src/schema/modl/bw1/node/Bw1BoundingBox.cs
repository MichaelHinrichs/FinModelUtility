using schema;


namespace modl.schema.modl.bw1.node {
  [BinarySchema]
  public partial class Bw1BoundingBox : IBiSerializable {
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float Z1 { get; set; }

    public float X2 { get; set; }
    public float Y2 { get; set; }
    public float Z2 { get; set; }
  }
}
