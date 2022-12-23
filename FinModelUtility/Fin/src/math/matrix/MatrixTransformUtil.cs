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

    public static IFinMatrix4x4 FromTranslation(float x, float y, float z)
      => new FinMatrix4x4(Matrix4x4.CreateTranslation(x, y, z));

    public static IFinMatrix4x4 FromRotation(IRotation rotation)
      => MatrixTransformUtil.FromRotation(QuaternionUtil.Create(rotation));

    public static IFinMatrix4x4 FromRotation(Quaternion rotation)
      => new FinMatrix4x4(Matrix4x4.CreateFromQuaternion(rotation));

    public static IFinMatrix4x4 FromScale(IScale scale)
      => new FinMatrix4x4(Matrix4x4.CreateScale(scale.X, scale.Y, scale.Z));

    public static IFinMatrix4x4 FromTrs(
        IPosition? translation,
        IRotation? rotation,
        IScale? scale)
      => MatrixTransformUtil.FromTrs(
          translation,
          rotation != null ? QuaternionUtil.Create(rotation) : null,
          scale);

    public static IFinMatrix4x4 FromTrs(
      IPosition? translation,
      Quaternion? rotation,
      IScale? scale)
      => FromTrs(translation, rotation, scale, new FinMatrix4x4());

    public static IFinMatrix4x4 FromTrs(
      IPosition? translation,
      Quaternion? rotation,
      IScale? scale,
      IFinMatrix4x4 dst) {
      dst.SetIdentity();

      if (translation != null) {
        dst.MultiplyInPlace(MatrixTransformUtil.FromTranslation(translation));
      }

      if (rotation != null) {
        dst.MultiplyInPlace(
          MatrixTransformUtil.FromRotation(rotation.Value));
      }

      if (scale != null) {
        dst.MultiplyInPlace(MatrixTransformUtil.FromScale(scale));
      }

      return dst;
    }

    public static IFinMatrix4x4 FromTrs(
        IPosition?[] translations,
        Quaternion?[] rotations,
        IScale?[] scales) {
      var matrix = MatrixTransformUtil.FromIdentity();

      foreach (var translation in translations) {
        if (translation != null) {
          matrix.MultiplyInPlace(MatrixTransformUtil.FromTranslation(translation));
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