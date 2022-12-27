using System;
using System.Collections.Generic;
using fin.math.matrix;
using fin.model;
using fin.util.asserts;
using System.Numerics;
using System.Runtime.CompilerServices;


namespace fin.math {
  using SystemMatrix = System.Numerics.Matrix4x4;

  public sealed class FinMatrix4x4 : IFinMatrix4x4 {
    internal SystemMatrix impl_;
    public SystemMatrix Impl => this.impl_;

    public static IReadOnlyFinMatrix4x4 IDENTITY =
      new FinMatrix4x4().SetIdentity();

    public FinMatrix4x4() {
      this.SetZero();
    }

    public FinMatrix4x4(IReadOnlyList<float> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this[i] = data[i];
      }
    }

    public FinMatrix4x4(IReadOnlyList<double> data) {
      Asserts.Equal(4 * 4, data.Count);
      for (var i = 0; i < 4 * 4; ++i) {
        this[i] = (float)data[i];
      }
    }

    public FinMatrix4x4(IReadOnlyFinMatrix4x4 other) => this.CopyFrom(other);
    public FinMatrix4x4(SystemMatrix other) => this.CopyFrom(other);

    public IFinMatrix4x4 Clone() => new FinMatrix4x4(this);

    public void CopyFrom(IReadOnlyFinMatrix4x4 other) {
      Asserts.Different(this, other, "Copying into same matrix!");

      if (other is FinMatrix4x4 otherImpl) {
        impl_ = otherImpl.Impl;
      } else {
        for (var r = 0; r < 4; ++r) {
          for (var c = 0; c < 4; ++c) {
            this[r, c] = other[r, c];
          }
        }
      }
    }

    public void CopyFrom(SystemMatrix other) {
      impl_ = other;
    }

    public IFinMatrix4x4 SetIdentity() {
      impl_ = SystemMatrix.Identity;
      return this;
    }

    public IFinMatrix4x4 SetZero() {
      impl_ = new SystemMatrix();
      return this;
    }

    public float this[int index] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Unsafe.Add(ref impl_.M11, index);
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => Unsafe.Add(ref impl_.M11, index) = value;
    }

    public float this[int row, int column] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => this[FinMatrix4x4.GetIndex_(row, column)];
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => this[FinMatrix4x4.GetIndex_(row, column)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
      if (other is FinMatrix4x4 otherImpl &&
          buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Add(impl_, otherImpl.impl_);
        return;
      }

      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          buffer[r, c] = this[r, c] + other[r, c];
        }
      }
    }


    // Matrix Multiplication
    public IFinMatrix4x4 CloneAndMultiply(IReadOnlyFinMatrix4x4 other)
      => this.Clone().MultiplyInPlace(other);

    public IFinMatrix4x4 MultiplyInPlace(IReadOnlyFinMatrix4x4 other) {
      this.MultiplyIntoBuffer(other, this);
      return this;
    }

    public void MultiplyIntoBuffer(
        IReadOnlyFinMatrix4x4 other,
        IFinMatrix4x4 buffer) {
      if (other is FinMatrix4x4 otherImpl &&
          buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Multiply(otherImpl.impl_, impl_);
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


    public IFinMatrix4x4 CloneAndMultiply(SystemMatrix other)
      => this.Clone().MultiplyInPlace(other);

    public IFinMatrix4x4 MultiplyInPlace(SystemMatrix other) {
      this.MultiplyIntoBuffer(other, this);
      return this;
    }

    public void MultiplyIntoBuffer(
        SystemMatrix other,
        IFinMatrix4x4 buffer) {
      if (buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Multiply(other, impl_);
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
      this.MultiplyIntoBuffer(other, this);
      return this;
    }

    public void MultiplyIntoBuffer(float other, IFinMatrix4x4 buffer) {
      if (buffer is FinMatrix4x4 bufferImpl) {
        bufferImpl.impl_ = SystemMatrix.Multiply(impl_, other);
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
      this.InvertIntoBuffer(this);
      return this;
    }

    public void InvertIntoBuffer(IFinMatrix4x4 buffer) {
      if (buffer is FinMatrix4x4 bufferImpl) {
        SystemMatrix.Invert(impl_, out bufferImpl.impl_);
        return;
      }

      SystemMatrix.Invert(impl_, out var invertedSystemMatrix);
      MatrixConversionUtil.CopySystemIntoFin(invertedSystemMatrix, buffer);
    }


    public IFinMatrix4x4 CloneAndTranspose()
      => this.Clone().TransposeInPlace();

    public IFinMatrix4x4 TransposeInPlace() {
      impl_ = Matrix4x4.Transpose(impl_);
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

    // Shamelessly copied from https://math.stackexchange.com/a/1463487
    public void CopyTranslationInto(IPosition dst) {
      var translation = impl_.Translation;
      dst.X = translation.X;
      dst.Y = translation.Y;
      dst.Z = translation.Z;
    }

    public void CopyRotationInto(out Quaternion dst) {
      this.Decompose(out _, out dst, out _);
      dst = -dst;
    }

    public void CopyScaleInto(IScale dst) {
      this.Decompose(out _, out _, out var scale);
      dst.X = scale.X;
      dst.Y = scale.Y;
      dst.Z = scale.Z;
    }

    public void Decompose(out Vector3 translation, out Quaternion rotation,
      out Vector3 scale) {
      Asserts.True(Matrix4x4.Decompose(impl_, out scale, out rotation, out translation), "Failed to decompose matrix!");
    }


    private const float ERROR = 0.0001f;

    public override bool Equals(object? obj)
      => ReferenceEquals(this, obj) || this.Equals(obj);

    public bool Equals(IReadOnlyFinMatrix4x4? other) {
      if (other == null) {
        return false;
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