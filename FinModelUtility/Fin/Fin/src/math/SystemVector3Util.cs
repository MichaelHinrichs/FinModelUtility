using System.Numerics;

namespace fin.math {
  public static class SystemVector3Util {
    public static Vector2 Xz(this Vector3 vec3) => new(vec3.X, vec3.Z);
  }
}