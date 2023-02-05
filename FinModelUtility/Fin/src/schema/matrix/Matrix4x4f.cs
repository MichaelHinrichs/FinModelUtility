using schema.binary;
using schema.binary.attributes.ignore;


namespace fin.schema.matrix {
  [BinarySchema]
  public partial class Matrix4x4f : IBiSerializable {
    public float[] Values { get; } = new float[4 * 4];

    [Ignore]
    public float this[int row, int column] {
      get => this.Values[4 * row + column];
      set => this.Values[4 * row + column] = value;
    }
  }
}
