using schema;


namespace fin.schema.vector {
  [Schema]
  public partial class Vector3f : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }
}
