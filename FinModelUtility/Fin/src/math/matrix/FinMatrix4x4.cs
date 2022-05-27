using System;

using fin.math.matrix;
using fin.util.asserts;


namespace fin.math {
  using SystemMatrix = System.Numerics.Matrix4x4;

  public class FinMatrix4x4 : IFinMatrix4x4 {
    private readonly double[] impl_ = new double[16];

    public FinMatrix4x4() { }

    public FinMatrix4x4(IReadOnlyFinMatrix4x4 other) => other.CopyInto(this);

    public IFinMatrix4x4 Clone() => new FinMatrix4x4(this);

    public void CopyInto(IFinMatrix4x4 other) {
      Asserts.Different(this, other, "Copying into same matrix!");

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          other[r, c] = this[r, c];
        }
      }
    }

    public IFinMatrix4x4 SetIdentity() {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] = (r == c) ? 1 : 0;
        }
      }
      return this;
    }

    public double this[int row, int column] {
      get => this.impl_[FinMatrix4x4.GetIndex_(row, column)];
      set => this.impl_[FinMatrix4x4.GetIndex_(row, column)] = value;
    }

    private static int GetIndex_(int row, int column) => 4 * row + column;


    // Addition
    public IFinMatrix4x4 CloneAndAdd(IReadOnlyFinMatrix4x4 other)
      => this.Clone().AddInPlace(other);

    public IFinMatrix4x4 AddInPlace(IReadOnlyFinMatrix4x4 other) {
      this.AddIntoBuffer(other, this);
      return this;
    }

    public void AddIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        IFinMatrix4x4 buffer) {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          buffer[r, c] = this[r, c] + other[r, c];
        }
      }
    }


    // Matrix Multiplication
    private static readonly FinMatrix4x4 SHARED_BUFFER = new();

    public IFinMatrix4x4 CloneAndMultiply(IReadOnlyFinMatrix4x4 other)
      => this.Clone().MultiplyInPlace(other);

    public IFinMatrix4x4 MultiplyInPlace(IReadOnlyFinMatrix4x4 other) {
      this.MultiplyIntoBuffer(other, FinMatrix4x4.SHARED_BUFFER);
      FinMatrix4x4.SHARED_BUFFER.CopyInto(this);
      return this;
    }

    public void MultiplyIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        IFinMatrix4x4 buffer) {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          var value = 0d;

          for (var i = 0; i < 4; ++i) {
            value += this[r, i] * other[i, c];
          }

          buffer[r, c] = value;
        }
      }
    }

    public IFinMatrix4x4 CloneAndMultiply(double other)
      => this.Clone().MultiplyInPlace(other);

    public IFinMatrix4x4 MultiplyInPlace(double other) {
      this.MultiplyIntoBuffer(other, this);
      return this;
    }

    public void MultiplyIntoBuffer(double other, IFinMatrix4x4 buffer) {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] *= other;
        }
      }
    }

    public IFinMatrix4x4 CloneAndInvert()
      => this.Clone().InvertInPlace();

    public IFinMatrix4x4 InvertInPlace() {
      this.InvertIntoBuffer(this);
      return this;
    }

    public void InvertIntoBuffer(IFinMatrix4x4 buffer) {
      // TODO: calculate this here

      var systemMatrix = new SystemMatrix();
      MatrixConversionUtil.CopyFinIntoSystem(this, ref systemMatrix);

      SystemMatrix.Invert(systemMatrix, out var invertedSystemMatrix);

      MatrixConversionUtil.CopySystemIntoFin(invertedSystemMatrix, buffer);
    }


    protected bool Equals(IReadOnlyFinMatrix4x4 other) {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          if (Math.Abs(this[r, c] - other[r, c]) > .0001) {
            return false;
          }
        }
      }
      return true;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }
      return Equals((IReadOnlyFinMatrix4x4)obj);
    }
  }
}