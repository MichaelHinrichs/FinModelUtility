using System.Numerics;
using fin.model;
using MathNet.Numerics.LinearAlgebra.Complex;


namespace fin.math.matrix {
  public static class MatrixTransformUtil {
    public static IReadOnlyFinMatrix4x4 IDENTITY { get; } =
      MatrixTransformUtil.FromIdentity();

    public static IFinMatrix4x4 FromIdentity()
      => new FinMatrix4x4().SetIdentity();

    public static IFinMatrix4x4 FromTranslation(Position translation)
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

    public static IFinMatrix4x4 FromScale(Scale scale)
      => FromScale(scale.X, scale.Y, scale.Z);

    public static IFinMatrix4x4 FromScale(float scale)
      => FromScale(scale, scale, scale);

    public static IFinMatrix4x4 FromScale(float scaleX,
                                          float scaleY,
                                          float scaleZ)
      => new FinMatrix4x4(Matrix4x4.CreateScale(scaleX, scaleY, scaleZ));

    public static IFinMatrix4x4 FromTrs(
        Position? translation,
        IRotation? rotation,
        Scale? scale)
      => MatrixTransformUtil.FromTrs(
          translation,
          rotation != null ? QuaternionUtil.Create(rotation) : null,
          scale);

    public static IFinMatrix4x4 FromTrs(
      Position? translation,
      Quaternion? rotation,
      Scale? scale)
      => FromTrs(translation, rotation, scale, new FinMatrix4x4());

    public static IFinMatrix4x4 FromTrs(
      Position? translation,
      Quaternion? rotation,
      Scale? scale,
      IFinMatrix4x4 dst) {
      dst.SetIdentity();

      if (translation != null) {
        dst.MultiplyInPlace(MatrixTransformUtil.FromTranslation(translation.Value));
      }

      if (rotation != null) {
        dst.MultiplyInPlace(
          MatrixTransformUtil.FromRotation(rotation.Value));
      }

      if (scale != null) {
        dst.MultiplyInPlace(MatrixTransformUtil.FromScale(scale.Value));
      }

      return dst;
    }

    public static IFinMatrix4x4 FromTrs(
        Position?[] translations,
        Quaternion?[] rotations,
        Scale?[] scales) {
      var matrix = MatrixTransformUtil.FromIdentity();

      foreach (var translation in translations) {
        if (translation != null) {
          matrix.MultiplyInPlace(MatrixTransformUtil.FromTranslation(translation.Value));
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
          matrix.MultiplyInPlace(MatrixTransformUtil.FromScale(scale.Value));
        }
      }

      return matrix;
    }
  }
}