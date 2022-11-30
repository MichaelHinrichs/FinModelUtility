using System;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class QuaternionUtilTests {
    [Test]
    public void ToEulerDegrees() {
      var degToRad = MathF.PI / 180;

      var x = 1 * 20 * degToRad;
      var y = 2 * 20 * degToRad;
      var z = 3 * 20 * degToRad;

      var q = QuaternionUtil.Create(x, y, z);

      var v = QuaternionUtil.ToEulerRadians(q);

      Assert.AreEqual(x, v.X, .0001);
      Assert.AreEqual(y, v.Y, .0001);
      Assert.AreEqual(z, v.Z, .0001);
    }
  }
}