using schema;


namespace fin.schema.vector {
  [BinarySchema]
  public partial class Vector3s : IBiSerializable {
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }

    public void Set(short x, short y, short z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }
  }
}
