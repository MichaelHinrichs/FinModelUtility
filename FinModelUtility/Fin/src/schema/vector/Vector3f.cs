using schema;


namespace fin.schema.vector {
  [BinarySchema]
  public partial class Vector3f : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public void Set(float x, float y, float z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }
  }
}
