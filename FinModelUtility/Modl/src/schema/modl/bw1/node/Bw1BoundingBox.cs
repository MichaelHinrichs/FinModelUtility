using schema;


namespace modl.schema.modl.bw1.node {
  [Schema]
  public partial class Bw1BoundingBox : IDeserializable {
    [EndianOrdered]
    private readonly string magic_ = "BBOX";

    private readonly uint size_ = 4 * 6;

    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float Z1 { get; set; }

    public float X2 { get; set; }
    public float Y2 { get; set; }
    public float Z2 { get; set; }
  }
}
