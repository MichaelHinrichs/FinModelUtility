using schema;

namespace mod.schema {
  public interface IVector2<T> : IBiSerializable {
    T X { get; set; }
    T Y { get; set; }

    string? ToString() => $"{this.X} {this.Y}";
  }

  [Schema]
  public partial class Vector2i : IVector2<uint> {
    public uint X { get; set; }
    public uint Y { get; set; }

    public Vector2i() { }
    public Vector2i(uint x, uint y) {
      this.X = x;
      this.Y = y;
    }
  }
}