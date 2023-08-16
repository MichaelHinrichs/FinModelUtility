using System;
using System.Numerics;

using fin.model;

namespace fin.math.matrix {
  public enum MatrixState {
    UNDEFINED,
    IDENTITY,
    ZERO,
  }

  public interface IFinMatrix4x4 : IReadOnlyFinMatrix4x4 {
    void CopyFrom(IReadOnlyFinMatrix4x4 other);
    void CopyFrom(Matrix4x4 other);

    IFinMatrix4x4 SetIdentity();
    IFinMatrix4x4 SetZero();

    new float this[int row, int column] { get; set; }

    IFinMatrix4x4 AddInPlace(IReadOnlyFinMatrix4x4 other);
    IFinMatrix4x4 MultiplyInPlace(IReadOnlyFinMatrix4x4 other);
    IFinMatrix4x4 MultiplyInPlace(Matrix4x4 other);
    IFinMatrix4x4 MultiplyInPlace(float other);
    IFinMatrix4x4 InvertInPlace();
    IFinMatrix4x4 TransposeInPlace();
  }

  public interface IReadOnlyFinMatrix4x4 : IEquatable<IReadOnlyFinMatrix4x4> {
    Matrix4x4 Impl { get; }

    IFinMatrix4x4 Clone();

    float this[int row, int column] { get; }

    IFinMatrix4x4 CloneAndAdd(IReadOnlyFinMatrix4x4 other);
    void AddIntoBuffer(IReadOnlyFinMatrix4x4 other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndMultiply(IReadOnlyFinMatrix4x4 other);
    void MultiplyIntoBuffer(IReadOnlyFinMatrix4x4 other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndMultiply(Matrix4x4 other);
    void MultiplyIntoBuffer(Matrix4x4 other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndMultiply(float other);
    void MultiplyIntoBuffer(float other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndInvert();
    void InvertIntoBuffer(IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndTranspose();
    void TransposeIntoBuffer(IFinMatrix4x4 buffer);

    void CopyTranslationInto(out Position dst);
    void CopyRotationInto(out Quaternion dst);
    void CopyScaleInto(out Scale dst);
    void Decompose(out Vector3 translation, out Quaternion rotation, out Vector3 scale);
  }
}