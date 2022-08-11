using schema;
using schema.attributes.ignore;


namespace fin.schema.matrix {
  [Schema]
  public partial class Matrix4x3f : IBiSerializable {
    public float[] Values { get; } = new float[4 * 3];

    [Ignore]
    public float this[int row, int column] {
      get => this.Values[3 * row + column];
      set => this.Values[3 * row + column] = value;
    }
  }
}
