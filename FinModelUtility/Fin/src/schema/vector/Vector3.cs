using schema;


namespace fin.schema.vector {
  public abstract class BVector3<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    public T this[int index] => index switch {
        0 => X,
        1 => Y,
        2 => Z,
    };

    public void Set(T x, T y, T z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }
  }

  [BinarySchema]
  public partial class Vector3f : BVector3<float>, IBiSerializable { }

  [BinarySchema]
  public partial class Vector3i : BVector3<int>, IBiSerializable { }

  [BinarySchema]
  public partial class Vector3s : BVector3<short>, IBiSerializable { }
}
