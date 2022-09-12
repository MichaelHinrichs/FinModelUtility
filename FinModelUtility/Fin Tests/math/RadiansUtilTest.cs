using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.math {
  [TestClass]
  public class RadiansUtilTest {
    [TestMethod]
    public void TestAngleDifference() {
      var pi = MathF.PI;

      AreEqual(0, RadiansUtil.angleDifference(0, 0));
      AreEqual(0, RadiansUtil.angleDifference(pi, pi));
      AreEqual(0, RadiansUtil.angleDifference(2 * pi, 0));
      AreEqual(0, RadiansUtil.angleDifference(4 * pi, 0));
      AreEqual(0, RadiansUtil.angleDifference(pi, -pi));

      AreEqual(pi, RadiansUtil.angleDifference(0, pi));
      AreEqual(-pi, RadiansUtil.angleDifference(pi, 0));

      AreEqual(pi / 2, RadiansUtil.angleDifference(pi / 4, -pi / 4));
      AreEqual(pi / 2, RadiansUtil.angleDifference(pi / 4, pi * 7 / 4));
      AreEqual(-pi / 2, RadiansUtil.angleDifference(-pi / 4, pi / 4));
      AreEqual(-pi / 2, RadiansUtil.angleDifference(pi * 7 / 4, pi / 4));
    }

    public void AreEqual(float expected, float actual) {
      Assert.AreEqual(expected, actual, .0001);
    }
  }
}