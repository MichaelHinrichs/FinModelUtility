using schema;


namespace fin.schema.vector {
  [Schema]
  public partial class Vector2i : IBiSerializable {
    public int X { get; set; }
    public int Y { get; set; }
  }
}
