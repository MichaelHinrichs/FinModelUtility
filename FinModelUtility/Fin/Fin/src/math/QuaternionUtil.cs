using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using fin.model;

using MathNet.Numerics;


namespace fin.math {
  public static class QuaternionUtil {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion Create(IRotation rotation)
      => QuaternionUtil.CreateZyx(rotation.XRadians,
                               rotation.YRadians,
                               rotation.ZRadians);

    public static Quaternion CreateZyx(
        float xRadians,
        float yRadians,
        float zRadians) {
      var q = Quaternion.Identity;

      if (!zRadians.AlmostEqual(0, .001)) {
        q *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, zRadians);
      }

      if (!yRadians.AlmostEqual(0, .001)) {
        q *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, yRadians);
      }

      if (!xRadians.AlmostEqual(0, .001)) {
        q *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, xRadians);
      }

      return Quaternion.Normalize(q);
    }

    public static Vector3 ToEulerRadians(Quaternion q) {
      if (q.IsIdentity) {
        return Vector3.Zero;
      }

      Vector3 angles = new();

      // roll / x
      var sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
      var cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
      angles.X = FinTrig.Atan2(sinr_cosp, cosr_cosp);

      // pitch / y
      var sinp = (float) (2 * (q.W * q.Y - q.Z * q.X));
      if (Math.Abs(sinp) >= 1) {
        angles.Y = MathF.CopySign(MathF.PI / 2, sinp);
      } else {
        angles.Y = FinTrig.Asin(sinp);
      }

      // yaw / z
      var siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
      var cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
      angles.Z = FinTrig.Atan2(siny_cosp, cosy_cosp);

      return angles;
    }
  }
}