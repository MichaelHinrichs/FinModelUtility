using System;

using fin.math.rotations;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class RadiansUtilTests {
    private const float PI = MathF.PI;

    [Test]
    [TestCase(0, 0, 0)]
    [TestCase(0, PI, PI)]
    [TestCase(0, 2 * PI, 0)]
    [TestCase(0, -2 * PI, 0)]
    [TestCase(0, 4 * PI, 0)]
    [TestCase(0, PI, -PI)]
    // Clockwise (positive)
    [TestCase(PI / 2, -PI / 4, PI / 4)]
    [TestCase(PI / 2, PI / 4, PI * 3 / 4)]
    [TestCase(PI / 2, PI * 3 / 4, -PI * 3 / 4)]
    [TestCase(PI / 2, PI * 7 / 4, PI / 4)]
    // Counterclockwise (Negative)
    [TestCase(-PI / 2, PI / 4, -PI / 4)]
    [TestCase(-PI / 2, PI * 3 / 4, PI / 4)]
    [TestCase(-PI / 2, -PI * 3 / 4, PI * 3 / 4)]
    [TestCase(-PI / 2, PI / 4, PI * 7 / 4)]
    public void TestCalculateRadiansTowardsExact(
        float expected,
        float from,
        float to)
      => AreEqual(expected, RadiansUtil.CalculateRadiansTowards(from, to));

    [Test]
    // Clockwise (positive)
    [TestCase(PI, 0, PI)]
    [TestCase(PI, PI, 2 * PI)]
    // Counterclockwise (Negative)
    [TestCase(-PI, PI, 0)]
    [TestCase(-PI, 2 * PI, PI)]
    public void TestCalculateRadiansTowardsHalfRotation(
        float expected,
        float from,
        float to)
      => AreEqual(expected, RadiansUtil.CalculateRadiansTowards(from, to));


    public void AreEqual(float expected, float actual)
      => Assert.AreEqual(expected, actual, .0001);
  }
}