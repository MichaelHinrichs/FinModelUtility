using System;
using System.Collections.Generic;
using fin.math.matrix;
using fin.util.asserts;
using System.Runtime.CompilerServices;


namespace fin.math {
  using SystemMatrix = System.Numerics.Matrix4x4;

  public class FinMatrix4x4 : IFinMatrix4x4 {
    private SystemMatrix impl_ = new();

    public FinMatrix4x4() {
      this.SetZero();
    }

    public FinMatrix4x4(IReadOnlyList<float> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this[i] = data[i];
      }
      this.UpdateState();
    }

    public FinMatrix4x4(IReadOnlyList<double> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this[i] = (float)data[i];
      }
      this.UpdateState();
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
      this.MatrixState = other.MatrixState;
    }

    public MatrixState MatrixState { get; private set; }
    public bool IsIdentity => this.MatrixState == MatrixState.IDENTITY;
    public bool IsZero => this.MatrixState == MatrixState.ZERO;


    public void UpdateState() {
      var isZero = true;
      var isIdentity = true;
      var error = .0001;

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          var isValueZero = Math.Abs(this[r, c]) < error;
          isZero &= isValueZero;

          var isValueIdentity =
              Math.Abs(this[r, c] - ((r == c) ? 1 : 0)) < error;
          isIdentity &= isValueIdentity;
        }
      }
      this.MatrixState = isZero ? MatrixState.ZERO :
                         isIdentity ? MatrixState.IDENTITY :
                         MatrixState.UNDEFINED;
    }

    public IFinMatrix4x4 SetIdentity() {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] = (r == c) ? 1 : 0;
        }
      }
      this.MatrixState = MatrixState.IDENTITY;
      return this;
    }

    public IFinMatrix4x4 SetZero() {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          this[r, c] = 0;
        }
      }
      this.MatrixState = MatrixState.ZERO;
      return this;
    }

    public float this[int index] {
      get => Unsafe.Add(ref this.impl_.M11, index);
      set {
        Unsafe.Add(ref this.impl_.M11, index) = value;
        this.MatrixState = MatrixState.UNDEFINED;
      }
    }

    public float this[int row, int column] {
      get => this[FinMatrix4x4.GetIndex_(row, column)];
      set => this[FinMatrix4x4.GetIndex_(row, column)] = value;
    }

    private static int GetIndex_(int row, int column) => 4 * row + column;


    // Addition
    public IFinMatrix4x4 CloneAndAdd(IReadOnlyFinMatrix4x4 other)
      => this.Clone().AddInPlace(other);

    public IFinMatrix4x4 AddInPlace(IReadOnlyFinMatrix4x4 other) {
      if (!other.IsZero) {
        this.AddIntoBuffer(other, this);
        this.MatrixState = MatrixState.UNDEFINED;
      }
      return this;
    }

    public void AddIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        IFinMatrix4x4 buffer) {
      if (this.IsZero) {
        buffer.CopyFrom(other);
        return;
      }
      if (other.IsZero) {
        buffer.CopyFrom(this);
        return;
      }

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
        this.MultiplyIntoBuffer(other, this);
      }
      return this;
    }

    public void MultiplyIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        IFinMatrix4x4 buffer) {
      if (this.IsIdentity) {
        buffer.CopyFrom(other);
        return;
      }
      if (other.IsIdentity) {
        buffer.CopyFrom(this);
        return;
      }
      if (this.IsZero || other.IsZero) {
        buffer.SetZero();
        return;
      }

      if (other is FinMatrix4x4 otherImpl &&
          buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Multiply(this.impl_, otherImpl.impl_);
        bufferImpl.MatrixState = MatrixState.UNDEFINED;
        return;
      }

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          var value = 0f;

          for (var i = 0; i < 4; ++i) {
            value += this[r, i] * other[i, c];
          }

          buffer[r, c] = value;
        }
      }
    }

    public IFinMatrix4x4 CloneAndMultiply(float other)
      => this.Clone().MultiplyInPlace(other);

    public IFinMatrix4x4 MultiplyInPlace(float other) {
      if (Math.Abs(other) < .0001) {
        this.SetZero();
        return this;
      }

      if (!this.IsZero && Math.Abs(other - 1) > ERROR) {
        this.MultiplyIntoBuffer(other, this);
      }

      return this;
    }

    public void MultiplyIntoBuffer(float other, IFinMatrix4x4 buffer) {
      if (this.IsZero || Math.Abs(other) < .0001f) {
        buffer.SetZero();
        return;
      }

      if (Math.Abs(other - 1) < ERROR) {
        buffer.CopyFrom(this);
        return;
      }

      if (buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Multiply(this.impl_, other);
        return;
      }

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          buffer[r, c] = this[r, c] * other;
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
      if (buffer is FinMatrix4x4 bufferImpl) {
        SystemMatrix.Invert(this.impl_, out bufferImpl.impl_);
        return;
      }

      SystemMatrix.Invert(this.impl_, out var invertedSystemMatrix);
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


    private const float ERROR = 0.0001f;

    public override bool Equals(object? obj)
      => ReferenceEquals(this, obj) || this.Equals(obj);

    public bool Equals(IReadOnlyFinMatrix4x4? other) {
      if (other == null) {
        return false;
      }

      if (this.IsIdentity && other.IsIdentity) {
        return true;
      }
      if (this.IsZero && other.IsZero) {
        return true;
      }

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          if (Math.Abs(this[r, c] - other[r, c]) > ERROR) {
            return false;
          }
        }
      }
      return true;
    }

    public override int GetHashCode() {
      int hash = 17;
      for (var i = 0; i < 16; ++i) {
        var value = this[i];
        value = MathF.Round(value / ERROR) * ERROR;
        hash = hash * 31 + value.GetHashCode();
      }
      return hash;
    }
  }
}