using System;
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

    public static Vector3 ToEulerRadians(Quaternion q) {
      Vector3 angles = new();

      // roll / x
      double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
      double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
      angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

      // pitch / y
      double sinp = 2 * (q.W * q.Y - q.Z * q.X);
      if (Math.Abs(sinp) >= 1) {
        angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
      } else {
        angles.Y = (float)Math.Asin(sinp);
      }

      // yaw / z
      double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
      double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
      angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

      return angles;
    }
  }
}