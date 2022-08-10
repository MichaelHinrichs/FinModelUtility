using schema;


namespace fin.schema.vector {
  [Schema]
  public partial class Vector2f : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
  }
}
