using schema;


namespace fin.schema.vector {
  public abstract class BVector4<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }
    public T W { get; set; }

    public T this[int index] => index switch {
        0 => X,
        1 => Y,
        2 => Z,
        3 => W,
    };

    public void Set(T x, T y, T z, T w) {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }
  }

  [BinarySchema]
  public sealed partial class Vector4f : BVector4<float>, IBiSerializable { }
}
