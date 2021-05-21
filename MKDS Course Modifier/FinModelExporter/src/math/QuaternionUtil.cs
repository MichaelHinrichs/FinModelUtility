using System.Numerics;

namespace fin.math {
  public static class QuaternionUtil {
    public static Quaternion Create(
        float xRadians,
        float yRadians,
        float zRadians) {
      var qz = Quaternion.CreateFromYawPitchRoll(0, 0, zRadians);
      var qy = Quaternion.CreateFromYawPitchRoll(yRadians, 0, 0);
      var qx = Quaternion.CreateFromYawPitchRoll(0, xRadians, 0);

      return Quaternion.Normalize(qz * qy * qx);
    }
  }
}