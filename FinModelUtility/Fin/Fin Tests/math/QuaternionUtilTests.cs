using System;
using System.Numerics;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class QuaternionUtilTests {
    private float PRECISION = .0001f;

    [Test]
    public void ToEulerRadians() {
      var degToRad = MathF.PI / 180;

      var x = 1 * 20 * degToRad;
      var y = 2 * 20 * degToRad;
      var z = 3 * 20 * degToRad;

      var q = QuaternionUtil.CreateZyx(x, y, z);

      var v = QuaternionUtil.ToEulerRadians(q);

      Assert.AreEqual(x, v.X, PRECISION);
      Assert.AreEqual(y, v.Y, PRECISION);
      Assert.AreEqual(z, v.Z, PRECISION);
    }

    [Test]
    public void ToEulerRadiansIdentity() {
      var v = QuaternionUtil.ToEulerRadians(Quaternion.Identity);

      Assert.AreEqual(0, v.X, PRECISION);
      Assert.AreEqual(0, v.Y, PRECISION);
      Assert.AreEqual(0, v.Z, PRECISION);
    }
  }
}