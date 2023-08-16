using System.Numerics;

namespace fin.math.matrix.three {
  public interface IFinMatrix3x2
      : IFinMatrix<IFinMatrix3x2, IReadOnlyFinMatrix3x2, Matrix3x2,
            Vector2, float, Vector2>,
        IReadOnlyFinMatrix3x2 { }

  public interface IReadOnlyFinMatrix3x2
      : IReadOnlyFinMatrix<IFinMatrix3x2, IReadOnlyFinMatrix3x2, Matrix3x2,
          Vector2, float, Vector2> { }
}