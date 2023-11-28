using System.Numerics;

namespace fin.math {
  public static class SystemVector4Util {
    public static Vector2 Xz(this Vector4 vec4) => new(vec4.X, vec4.Z);
    public static Vector2 Xy(this Vector4 vec4) => new(vec4.X, vec4.Y);

    public static Vector3 Xyz(this Vector4 vec4) => new(vec4.X, vec4.Y, vec4.Z);

    public static Vector4 Yzwx(this Vector4 vec4)
      => new(vec4.Y, vec4.Z, vec4.W, vec4.X);
  }
}