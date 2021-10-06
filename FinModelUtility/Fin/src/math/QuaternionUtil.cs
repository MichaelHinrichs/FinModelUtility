using System.Numerics;

using fin.model;

namespace fin.math {
  public static class QuaternionUtil {
    public static Quaternion Create(IRotation rotation)
      => QuaternionUtil.Create(rotation.XRadians,
                               rotation.YRadians,
                               rotation.ZRadians);

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