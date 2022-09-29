using System.Numerics;
using System.Runtime.CompilerServices;
using Vector4 = System.Numerics.Vector4;


namespace fin.math.matrix {
  public class FinVector4 {
    private Vector4 impl_ = new();

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
      get => Unsafe.Add(ref this.impl_.X, index);
      set => Unsafe.Add(ref this.impl_.X, index) = value;
    }

    public float X {
      get => this.impl_.X;
      set => this.impl_.X = value;
    }

    public float Y {
      get => this.impl_.Y;
      set => this.impl_.Y = value;
    }

    public float Z {
      get => this.impl_.Z;
      set => this.impl_.Z = value;
    }

    public float W {
      get => this.impl_.W;
      set => this.impl_.W = value;
    }


    // Normalizing
    public float Length => this.impl_.Length();

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
      if (other is FinMatrix4x4 otherImpl) {
        buffer.impl_ = Vector4.Transform(this.impl_, Matrix4x4.Transpose(otherImpl.impl_));
        return;
      }

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