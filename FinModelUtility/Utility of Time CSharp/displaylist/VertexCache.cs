using UoT.displaylist;

namespace UoT {
  public struct Vertex : IVertex {
    public bool Populated;

    public double X;
    public double Y;
    public double Z;

    public short U { get; set; }
    public short V { get; set; }

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public float NormalX { get; set; }
    public float NormalY { get; set; }
    public float NormalZ { get; set; }
  } 

  public class VertexCache {
    private readonly Vertex[] impl_;

    public VertexCache() {
      this.impl_ = new Vertex[32];
    }

    public void Reset() {
      for (var i = 0; i < this.impl_.Length; ++i) {
        this.impl_[i] = default;
      }
    }

    public Vertex this[int index] {
      get => this.impl_[index];
      set => this.impl_[index] = value;
    }
  }
}
