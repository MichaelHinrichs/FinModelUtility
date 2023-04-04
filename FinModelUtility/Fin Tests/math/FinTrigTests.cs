using System;

using NUnit.Framework;


namespace fin.math {
  public class FinTrigTests {
    [Test]
    [TestCase(-.5f * MathF.PI)]
    [TestCase(0)]
    [TestCase(.5f * MathF.PI)]
    [TestCase(MathF.PI)]
    [TestCase(1.5f * MathF.PI)]
    [TestCase(2 * MathF.PI)]
    [TestCase(2.5f * MathF.PI)]
    public void TestSin(float radians)
      => Assert.AreEqual(Math.Sin(radians), FinTrig.Sin(radians), FinTrig.PRECISION);

    [Test]
    [TestCase(-.5f * MathF.PI)]
    [TestCase(0)]
    [TestCase(.5f * MathF.PI)]
    [TestCase(MathF.PI)]
    [TestCase(1.5f * MathF.PI)]
    [TestCase(2 * MathF.PI)]
    [TestCase(2.5f * MathF.PI)]
    public void TestCos(float radians)
      => Assert.AreEqual(Math.Cos(radians), FinTrig.Cos(radians), FinTrig.PRECISION);

    [Test]
    [TestCase(-1)]
    [TestCase(-.5f)]
    [TestCase(0)]
    [TestCase(.5f)]
    [TestCase(1)]
    public void TestAsin(float radians)
      => Assert.AreEqual(Math.Asin(radians), FinTrig.Asin(radians), FinTrig.PRECISION);

    [Test]
    [TestCase(-1)]
    [TestCase(-.5f)]
    [TestCase(0)]
    [TestCase(.5f)]
    [TestCase(1)]
    public void TestAcos(float radians)
      => Assert.AreEqual(Math.Acos(radians), FinTrig.Acos(radians), FinTrig.PRECISION);

    [Test]
    [TestCase(-1, -1)]
    [TestCase(-1, 1)]
    [TestCase(1, -1)]
    [TestCase(1, 1)]
    public void TestAtan2(float x, float y)
      => Assert.AreEqual(Math.Atan2(y, x), FinTrig.Atan2(y, x), FinTrig.PRECISION);
  }
}