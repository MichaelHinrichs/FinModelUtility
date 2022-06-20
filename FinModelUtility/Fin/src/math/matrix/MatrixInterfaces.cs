using System;


namespace fin.math.matrix {
  public interface IFinMatrix4x4 : IReadOnlyFinMatrix4x4 {
    void CopyFrom(IReadOnlyFinMatrix4x4 other);
    
    IFinMatrix4x4 SetIdentity();
    void UpdateIsIdentity();

    new double this[int row, int column] { get; set; }

    IFinMatrix4x4 AddInPlace(IReadOnlyFinMatrix4x4 other);
    IFinMatrix4x4 MultiplyInPlace(IReadOnlyFinMatrix4x4 other);
    IFinMatrix4x4 MultiplyInPlace(double other);
  }

  public interface IReadOnlyFinMatrix4x4 {
    bool IsIdentity { get; }

    IFinMatrix4x4 Clone();

    double this[int row, int column] { get; }

    IFinMatrix4x4 CloneAndAdd(IReadOnlyFinMatrix4x4 other);
    void AddIntoBuffer(IReadOnlyFinMatrix4x4 other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndMultiply(IReadOnlyFinMatrix4x4 other);
    void MultiplyIntoBuffer(IReadOnlyFinMatrix4x4 other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndMultiply(double other);
    void MultiplyIntoBuffer(double other, IFinMatrix4x4 buffer);

    IFinMatrix4x4 CloneAndInvert();
    IFinMatrix4x4 InvertInPlace();
    void InvertIntoBuffer(IFinMatrix4x4 buffer);
  }
}