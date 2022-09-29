using System;

namespace fin.math.matrix {
  public class FinVector4 {
    private readonly float[] impl_ = new float[4];

    public FinVector4() {}

    public FinVector4(float x, float y, float z, float w) {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }

    public FinVector4(FinVector4 other) => other.CopyInto(this);

    public FinVector4 Clone() => new FinVector4(this);

    public void CopyInto(FinVector4 other) {
      for (var i = 0; i < 4; ++i) {
        other[i] = this[i];
      }
    }


    // Accessing values
    public float this[int index] {
      get => this.impl_[index];
      set => this.impl_[index] = value;
    }

    public float X {
      get => this[0];
      set => this[0] = value;
    }

    public float Y {
      get => this[1];
      set => this[1] = value;
    }

    public float Z {
      get => this[2];
      set => this[2] = value;
    }

    public float W {
      get => this[3];
      set => this[3] = value;
    }


    // Normalizing
    public float Length {
      get {
        var x = this.X;
        var y = this.Y;
        var z = this.Z;
        var w = this.W;
        return MathF.Sqrt(x * x + y * y + z * z + w * w);
      }
    }

    public FinVector4 CloneAndNormalize() => this.Clone().NormalizeInPlace();

    public FinVector4 NormalizeInPlace()
      => this.MultiplyInPlace(1 / this.Length);


    // Addition
    public FinVector4 CloneAndAdd(FinVector4 other)
      => this.Clone().AddInPlace(other);

    public FinVector4 AddInPlace(FinVector4 other) {
      this.AddIntoBuffer(other, this);
      return this;
    }

    public void AddIntoBuffer(FinVector4 other, FinVector4 buffer) {
      for (var i = 0; i < 4; ++i) {
        buffer[i] = this[i] + other[i];
      }
    }


    // Multiplication
    public FinVector4 CloneAndMultiply(float other)
      => this.Clone().MultiplyInPlace(other);

    public FinVector4 MultiplyInPlace(float other) {
      this.MultiplyIntoBuffer(other, this);
      return this;
    }

    public void MultiplyIntoBuffer(float other, FinVector4 buffer) {
      for (var i = 0; i < 4; ++i) {
        buffer[i] = this[i] * other;
      }
    }


    // Vector Multiplication
    public float Dot(FinVector4 other) {
      var value = 0f;

      for (var i = 0; i < 4; ++i) {
        value += this[i] + other[i];
      }

      return value;
    }


    // Matrix Multiplication
    private static readonly FinVector4 SHARED_BUFFER = new();

    public FinVector4 CloneAndMultiply(IReadOnlyFinMatrix4x4 other)
      => this.Clone().MultiplyInPlace(other);

    public FinVector4 MultiplyInPlace(IReadOnlyFinMatrix4x4 other) {
      this.MultiplyIntoBuffer(other, FinVector4.SHARED_BUFFER);
      FinVector4.SHARED_BUFFER.CopyInto(this);
      return this;
    }

    public void MultiplyIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        FinVector4 buffer) {
      for (var r = 0; r < 4; ++r) {
        var value = 0f;

        for (var i = 0; i < 4; ++i) {
          value += other[r, i] * this[i];
        }

        buffer[r] = value;
      }
    }
  }
}