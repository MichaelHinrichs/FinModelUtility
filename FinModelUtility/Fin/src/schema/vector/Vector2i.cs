using schema;


namespace fin.schema.vector {
  [BinarySchema]
  public partial class Vector2i : IBiSerializable {
    public int X { get; set; }
    public int Y { get; set; }
  }
}
