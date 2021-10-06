using System.Numerics;

using fin.model;

namespace fin.math.matrix {
  public static class MatrixTransformUtil {
    public static IReadOnlyFinMatrix4x4 IDENTITY { get; } =
      MatrixTransformUtil.FromIdentity();

    public static IFinMatrix4x4 FromIdentity()
      => new FinMatrix4x4().SetIdentity();

    public static IFinMatrix4x4 FromTranslation(IPosition translation)
      => MatrixTransformUtil.FromTranslation(
          translation.X,
          translation.Y,
          translation.Z);

    public static IFinMatrix4x4 FromTranslation(float x, float y, float z) {
      var matrix = MatrixTransformUtil.FromIdentity();

      matrix[0, 3] = x;
      matrix[1, 3] = y;
      matrix[2, 3] = z;

      return matrix;
    }

    public static IFinMatrix4x4 FromRotation(IRotation rotation)
      => MatrixTransformUtil.FromRotation(QuaternionUtil.Create(rotation));

    public static IFinMatrix4x4 FromRotation(Quaternion rotation) {
      var qx = rotation.X;
      var qy = rotation.Y;
      var qz = rotation.Z;
      var qw = rotation.W;

      var matrix = new FinMatrix4x4();

      matrix[0, 0] = 1.0 - 2.0 * qy * qy - 2.0 * qz * qz;
      matrix[0, 1] = 2.0 * qx * qy - 2.0 * qz * qw;
      matrix[0, 2] = 2.0 * qx * qz + 2.0 * qy * qw;
      matrix[0, 3] = 0.0;

      matrix[1, 0] = 2.0 * qx * qy + 2.0 * qz * qw;
      matrix[1, 1] = 1.0 - 2.0 * qx * qx - 2.0 * qz * qz;
      matrix[1, 2] = 2.0 * qy * qz - 2.0 * qx * qw;
      matrix[1, 3] = 0.0;

      matrix[2, 0] = 2.0 * qx * qz - 2.0 * qy * qw;
      matrix[2, 1] = 2.0 * qy * qz + 2.0 * qx * qw;
      matrix[2, 2] = 1.0 - 2.0 * qx * qx - 2.0 * qy * qy;
      matrix[2, 3] = 0.0;

      matrix[3, 0] = 0;
      matrix[3, 1] = 0;
      matrix[3, 2] = 0;
      matrix[3, 3] = 1;

      return matrix;
    }

    public static IFinMatrix4x4 FromScale(IScale scale)
      => new FinMatrix4x4 {
          [0, 0] = scale.X,
          [1, 1] = scale.Y,
          [2, 2] = scale.Z,
          [3, 3] = 1,
      };

    public static IFinMatrix4x4 FromTrs(
        IPosition? position,
        IRotation? rotation,
        IScale? scale)
      => MatrixTransformUtil.FromTrs(
          position,
          rotation != null ? QuaternionUtil.Create(rotation) : null,
          scale);

    public static IFinMatrix4x4 FromTrs(
        IPosition? position,
        Quaternion? rotation,
        IScale? scale) {
      var matrix = MatrixTransformUtil.FromIdentity();

      if (position != null) {
        matrix.MultiplyInPlace(MatrixTransformUtil.FromTranslation(position));
      }

      if (rotation != null) {
        matrix.MultiplyInPlace(
            MatrixTransformUtil.FromRotation(rotation.Value));
      }

      if (scale != null) {
        matrix.MultiplyInPlace(MatrixTransformUtil.FromScale(scale));
      }

      return matrix;
    }
  }
}