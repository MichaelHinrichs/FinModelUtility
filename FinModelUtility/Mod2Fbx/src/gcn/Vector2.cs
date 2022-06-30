using System.IO;

using schema;

namespace mod.gcn {
  public interface IVector2<T> : IBiSerializable {
    T X { get; set; }
    T Y { get; set; }

    string? ToString() => $"{this.X} {this.Y}";
  }

  [Schema]
  public partial class Vector2f : IVector2<float> {
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2f() {}
    public Vector2f(float x, float y) {
      this.X = x;
      this.Y = y;
    }
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