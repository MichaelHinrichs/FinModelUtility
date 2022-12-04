using schema;


namespace fin.schema.vector {
  public abstract class BVector2<T> {
    public T X { get; set; }
    public T Y { get; set; }

    public T this[int index] => index switch {
        0 => X,
        1 => Y,
    };

    public void Set(T x, T y) {
      this.X = x; 
      this.Y = y;
    }
  }

  [BinarySchema]
  public partial class Vector2f : BVector2<float>, IBiSerializable { }

  [BinarySchema]
  public partial class Vector2i : BVector2<int>, IBiSerializable { }

  [BinarySchema]
  public partial class Vector2s : BVector2<short>, IBiSerializable { }
}