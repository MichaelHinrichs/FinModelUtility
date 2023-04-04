using System;
using System.Numerics;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class QuaternionUtilTests {
    [Test]
    public void ToEulerRadians() {
      var degToRad = MathF.PI / 180;

      var x = 1 * 20 * degToRad;
      var y = 2 * 20 * degToRad;
      var z = 3 * 20 * degToRad;

      var q = QuaternionUtil.Create(x, y, z);

      var v = QuaternionUtil.ToEulerRadians(q);

      Assert.AreEqual(x, v.X, FinTrig.PRECISION);
      Assert.AreEqual(y, v.Y, FinTrig.PRECISION);
      Assert.AreEqual(z, v.Z, FinTrig.PRECISION);
    }

    [Test]
    public void ToEulerRadiansIdentity() {
      var v = QuaternionUtil.ToEulerRadians(Quaternion.Identity);

      Assert.AreEqual(0, v.X, FinTrig.PRECISION);
      Assert.AreEqual(0, v.Y, FinTrig.PRECISION);
      Assert.AreEqual(0, v.Z, FinTrig.PRECISION);
    }
  }
}