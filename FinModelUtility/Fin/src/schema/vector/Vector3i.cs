using schema;


namespace fin.schema.vector {
  [BinarySchema]
  public partial class Vector3i : IBiSerializable {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public void Set(int x, int y, int z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }
  }
}
