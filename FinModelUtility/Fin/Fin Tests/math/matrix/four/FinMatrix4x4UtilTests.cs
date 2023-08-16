using fin.math.rotations;
using fin.model;
using fin.util.asserts;

using NUnit.Framework;

namespace fin.math.matrix.four {
  public class FinMatrix4x4UtilTests {
    [Test]
    public void TestTranslation() {
      var expectedTranslation = new Position(2, 3, 4);

      var matrix = FinMatrix4x4Util.FromTranslation(
          expectedTranslation);

      matrix.CopyTranslationInto(out var actualTranslation);

      Assert.AreEqual(expectedTranslation, actualTranslation);
    }

    [Test]
    public void TestRotation() {
      var expectedRotation = QuaternionUtil.CreateZyx(1.2f, 2.3f, 3.4f);

      var matrix = FinMatrix4x4Util.FromRotation(
          expectedRotation);

      matrix.CopyRotationInto(out var actualRotation);

      Asserts.IsRoughly(expectedRotation.X, actualRotation.X);
      Asserts.IsRoughly(expectedRotation.Y, actualRotation.Y);
      Asserts.IsRoughly(expectedRotation.Z, actualRotation.Z);
      Asserts.IsRoughly(expectedRotation.W, actualRotation.W);
    }

    [Test]
    public void TestScale() {
      var expectedScale = new Scale(3, 4, 5);

      var matrix = FinMatrix4x4Util.FromScale(
          expectedScale);

      matrix.CopyScaleInto(out var actualScale);

      Assert.AreEqual(expectedScale, actualScale);
    }


    [Test]
    public void TestTrs() {
      var expectedTranslation = new Position(2, 3, 4);
      var expectedRotation = QuaternionUtil.CreateZyx(1.2f, 2.3f, 3.4f);
      var expectedScale = new Scale(3, 4, 5);

      var trs = FinMatrix4x4Util.FromTrs(
          expectedTranslation,
          expectedRotation,
          expectedScale);

      trs.CopyTranslationInto(out var actualTranslation);
      trs.CopyRotationInto(out var actualRotation);
      trs.CopyScaleInto(out var actualScale);

      Assert.AreEqual(expectedTranslation, actualTranslation);

      Assert.AreEqual(expectedScale, actualScale);

      Asserts.IsRoughly(expectedRotation.X, actualRotation.X);
      Asserts.IsRoughly(expectedRotation.Y, actualRotation.Y);
      Asserts.IsRoughly(expectedRotation.Z, actualRotation.Z);
      Asserts.IsRoughly(expectedRotation.W, actualRotation.W);
    }
  }
}