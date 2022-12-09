using System.Numerics;
using fin.model;
using MathNet.Numerics.LinearAlgebra.Complex;


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

      matrix.UpdateState();

      return matrix;
    }

    public static IFinMatrix4x4 FromRotation(IRotation rotation)
      => MatrixTransformUtil.FromRotation(QuaternionUtil.Create(rotation));

    public static IFinMatrix4x4 FromRotation(Quaternion rotation)
      => new FinMatrix4x4(Matrix4x4.CreateFromQuaternion(rotation))
          .TransposeInPlace();

    public static IFinMatrix4x4 FromScale(IScale scale) {
      var matrix = new FinMatrix4x4 {
          [0, 0] = scale.X, [1, 1] = scale.Y, [2, 2] = scale.Z, [3, 3] = 1,
      };
      matrix.UpdateState();
      return matrix;
    }

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

    public static IFinMatrix4x4 FromTrs(
        IPosition?[] positions,
        Quaternion?[] rotations,
        IScale?[] scales) {
      var matrix = MatrixTransformUtil.FromIdentity();

      foreach (var position in positions) {
        if (position != null) {
          matrix.MultiplyInPlace(MatrixTransformUtil.FromTranslation(position));
        }
      }

      foreach (var rotation in rotations) {
        if (rotation != null) {
          matrix.MultiplyInPlace(
              MatrixTransformUtil.FromRotation(rotation.Value));
        }
      }

      foreach (var scale in scales) {
        if (scale != null) {
          matrix.MultiplyInPlace(MatrixTransformUtil.FromScale(scale));
        }
      }

      return matrix;
    }
  }
}