using System;
using System.Numerics;

using fin.math.rotations;

using MathNet.Numerics;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class QuaternionUtilTests {
    private float PRECISION = .01f;

    [Test]
    [TestCase(0, 0, 0, 0, 0, 0, 1)]
    [TestCase(2 * MathF.PI, 2 * MathF.PI, 2 * MathF.PI, 0, 0, 0, -1)]
    [TestCase(1, 2, 3, -0.71f, 0.31f, 0.44f, 0.43f)]
    [TestCase(2, 3, 4, -.51f, -.17f, .38f, .74f)]
    public void TestCreateZyx(float xRadians,
                              float yRadians,
                              float zRadians,
                              float expectedQX,
                              float expectedQY,
                              float expectedQZ,
                              float expectedQW) {
      var actualQuaternion = QuaternionUtil.CreateZyx(xRadians, yRadians, zRadians);

      if (!actualQuaternion.X.AlmostEqual(expectedQX, this.PRECISION) ||
          !actualQuaternion.Y.AlmostEqual(expectedQY, this.PRECISION) ||
          !actualQuaternion.Z.AlmostEqual(expectedQZ, this.PRECISION) ||
          !actualQuaternion.W.AlmostEqual(expectedQW, this.PRECISION)) {
        Assert.Fail($"Expected {actualQuaternion} to equal {new Quaternion(expectedQX, expectedQY, expectedQZ, expectedQW)}");
      }
    }

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