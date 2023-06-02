using System.Numerics;
using System.Runtime.CompilerServices;

using fin.model;


namespace fin.math.matrix {
  public static class SystemMatrixUtil {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromTranslation(Position translation)
      => SystemMatrixUtil.FromTranslation(translation.X,
                                          translation.Y,
                                          translation.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromTranslation(float x, float y, float z)
      => Matrix4x4.CreateTranslation(x, y, z);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromRotation(IRotation rotation)
      => SystemMatrixUtil.FromRotation(QuaternionUtil.Create(rotation));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromRotation(Quaternion rotation)
      => Matrix4x4.CreateFromQuaternion(rotation);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromScale(Scale scale)
      => SystemMatrixUtil.FromScale(scale.X, scale.Y, scale.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromScale(float scale)
      => SystemMatrixUtil.FromScale(scale, scale, scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromScale(float scaleX, float scaleY, float scaleZ)
      => Matrix4x4.CreateScale(scaleX, scaleY, scaleZ);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromTrs(
        Position? translation,
        IRotation? rotation,
        Scale? scale)
      => SystemMatrixUtil.FromTrs(
          translation,
          rotation != null ? QuaternionUtil.Create(rotation) : null,
          scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 FromTrs(
        Position? translation,
        Quaternion? rotation,
        Scale? scale) {
      var dst = Matrix4x4.Identity;

      if (translation != null) {
        dst = Matrix4x4.Multiply(
            SystemMatrixUtil.FromTranslation(translation.Value),
            dst);
      }

      if (rotation != null) {
        dst = Matrix4x4.Multiply(
            SystemMatrixUtil.FromRotation(rotation.Value),
            dst);
      }

      if (scale != null) {
        dst = Matrix4x4.Multiply(SystemMatrixUtil.FromScale(scale.Value),
                                 dst);
      }

      return dst;
    }
  }
}