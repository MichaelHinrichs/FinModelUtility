using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace fin.math {
  public static class MatrixUtil {
    public static Matrix<double> Identity { get; } =
      new DiagonalMatrix(4, 4, 1);
  }
}