using schema;


namespace fin.schema.vector {
  [BinarySchema]
  public partial class Vector2s : IBiSerializable {
    public short X { get; set; }
    public short Y { get; set; }
  }
}
