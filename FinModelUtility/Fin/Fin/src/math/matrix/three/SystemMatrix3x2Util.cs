using System.Numerics;
using System.Runtime.CompilerServices;

namespace fin.math.matrix.three {
  public static class SystemMatrix3x2Util {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromTranslation(Vector2 translation)
      => FromTranslation(translation.X, translation.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromTranslation(float x, float y)
      => Matrix3x2.CreateTranslation(x, y);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromRotation(float rotation)
      => Matrix3x2.CreateRotation(rotation);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromScale(float scale)
      => SystemMatrix3x2Util.FromScale(scale, scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromScale(Vector2 scale)
      => FromScale(scale.X, scale.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromScale(float scaleX, float scaleY)
      => Matrix3x2.CreateScale(scaleX, scaleY);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromSkewXRadians(float skewXRadians)
      => Matrix3x2.CreateSkew(skewXRadians, 0);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 FromTrss(
        Vector2? translation,
        float? rotation,
        Vector2? scale,
        float? skewXRadians) {
      var dst = Matrix3x2.Identity;

      if (translation != null) {
        dst = Matrix3x2.Multiply(FromTranslation(translation.Value), dst);
      }

      if (rotation != null) {
        dst = Matrix3x2.Multiply(FromRotation(rotation.Value), dst);
      }

      if (scale != null) {
        dst = Matrix3x2.Multiply(FromScale(scale.Value), dst);
      }

      if (skewXRadians != null) {
        dst = Matrix3x2.Multiply(FromSkewXRadians(skewXRadians.Value), dst);
      }

      return dst;
    }
  }
}