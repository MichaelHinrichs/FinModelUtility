using System;
using System.Collections.Generic;

using fin.math.matrix;
using fin.util.asserts;


namespace fin.math {
  using SystemMatrix = System.Numerics.Matrix4x4;

  public class FinMatrix4x4 : IFinMatrix4x4 {
    private readonly double[] impl_ = new double[16];

    public FinMatrix4x4() { }

    public FinMatrix4x4(IReadOnlyList<float> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this.impl_[i] = data[i];
      }
      this.UpdateIsIdentity();
    }

    public FinMatrix4x4(IReadOnlyList<double> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this.impl_[i] = data[i];
      }
      this.UpdateIsIdentity();
    }

    public FinMatrix4x4(IReadOnlyFinMatrix4x4 other) => this.CopyFrom(other);

    public IFinMatrix4x4 Clone() => new FinMatrix4x4(this);

    public void CopyFrom(IReadOnlyFinMatrix4x4 other) {
      Asserts.Different(this, other, "Copying into same matrix!");

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] = other[r, c];
        }
      }
      this.IsIdentity = other.IsIdentity;
    }

    public bool IsIdentity { get; private set; }

    public void UpdateIsIdentity() {
      var isIdentity = true;
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          var isValueCorrect =
              Math.Abs(this[r, c] - ((r == c) ? 1 : 0)) < .0001;
          isIdentity &= isValueCorrect;
        }
      }
      this.IsIdentity = isIdentity;
    }

    public IFinMatrix4x4 SetIdentity() {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] = (r == c) ? 1 : 0;
        }
      }
      this.IsIdentity = true;
      return this;
    }

    public double this[int row, int column] {
      get => this.impl_[FinMatrix4x4.GetIndex_(row, column)];
      set {
        this.impl_[FinMatrix4x4.GetIndex_(row, column)] = value;
        this.IsIdentity = false;
      }
    }

    private static int GetIndex_(int row, int column) => 4 * row + column;


    // Addition
    public IFinMatrix4x4 CloneAndAdd(IReadOnlyFinMatrix4x4 other)
      => this.Clone().AddInPlace(other);

    public IFinMatrix4x4 AddInPlace(IReadOnlyFinMatrix4x4 other) {
      this.AddIntoBuffer(other, this);
      this.IsIdentity = false;
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
      if (!other.IsIdentity) {
        this.MultiplyIntoBuffer(other, FinMatrix4x4.SHARED_BUFFER);
        this.CopyFrom(FinMatrix4x4.SHARED_BUFFER);
      }
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
      if (Math.Abs(other - 1) > .0001) {
        this.MultiplyIntoBuffer(other, this);
      }
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
      if (!this.IsIdentity) {
        this.InvertIntoBuffer(this);
      }
      return this;
    }

    public void InvertIntoBuffer(IFinMatrix4x4 buffer) {
      // TODO: calculate this here

      var systemMatrix = new SystemMatrix();
      MatrixConversionUtil.CopyFinIntoSystem(this, ref systemMatrix);

      SystemMatrix.Invert(systemMatrix, out var invertedSystemMatrix);

      MatrixConversionUtil.CopySystemIntoFin(invertedSystemMatrix, buffer);
    }


    public IFinMatrix4x4 CloneAndTranspose()
      => this.Clone().TransposeInPlace();

    public IFinMatrix4x4 TransposeInPlace() {
      if (!this.IsIdentity) {
        this.TransposeIntoBuffer(FinMatrix4x4.SHARED_BUFFER);
        this.CopyFrom(FinMatrix4x4.SHARED_BUFFER);
      }
      return this;
    }

    public void TransposeIntoBuffer(IFinMatrix4x4 buffer) {
      Asserts.Different(this, buffer);
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          buffer[r, c] = this[c, r];
        }
      }
    }


    protected bool Equals(IReadOnlyFinMatrix4x4 other) {
      if (this.IsIdentity && other.IsIdentity) {
        return true;
      }

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
      return Equals((IReadOnlyFinMatrix4x4) obj);
    }
  }
}