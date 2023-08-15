using fin.math.matrix;
using fin.math.rotations;
using fin.model;

using NUnit.Framework;


namespace fin.math {
  public class FinMatrixUtilTests {
    [Test]
    public void TestTranslation() {
      var expectedTranslation = new Position(2, 3, 4);

      var matrix = FinMatrixUtil.FromTranslation(
          expectedTranslation);

      matrix.CopyTranslationInto(out var actualTranslation);

      Assert.AreEqual(expectedTranslation, actualTranslation);
    }

    [Test]
    public void TestRotation() {
      var expectedRotation = QuaternionUtil.CreateZyx(1.2f, 2.3f, 3.4f);

      var matrix = FinMatrixUtil.FromRotation(
          expectedRotation);

      matrix.CopyRotationInto(out var actualRotation);

      var error = .0001;
      Assert.AreEqual(expectedRotation.X, actualRotation.X, error);
      Assert.AreEqual(expectedRotation.Y, actualRotation.Y, error);
      Assert.AreEqual(expectedRotation.Z, actualRotation.Z, error);
      Assert.AreEqual(expectedRotation.W, actualRotation.W, error);
    }

    [Test]
    public void TestScale() {
      var expectedScale = new Scale(3, 4, 5);

      var matrix = FinMatrixUtil.FromScale(
          expectedScale);

      matrix.CopyScaleInto(out var actualScale);

      Assert.AreEqual(expectedScale, actualScale);
    }


    [Test]
    public void TestTrs() {
      var expectedTranslation = new Position(2, 3, 4);
      var expectedRotation = QuaternionUtil.CreateZyx(1.2f, 2.3f, 3.4f);
      var expectedScale = new Scale(3, 4, 5);

      var trs = FinMatrixUtil.FromTrs(
          expectedTranslation,
          expectedRotation,
          expectedScale);

      trs.CopyTranslationInto(out var actualTranslation);
      trs.CopyRotationInto(out var actualRotation);
      trs.CopyScaleInto(out var actualScale);

      Assert.AreEqual(expectedTranslation, actualTranslation);

      Assert.AreEqual(expectedScale, actualScale);

      var error = .0001;
      Assert.AreEqual(expectedRotation.X, actualRotation.X, error);
      Assert.AreEqual(expectedRotation.Y, actualRotation.Y, error);
      Assert.AreEqual(expectedRotation.Z, actualRotation.Z, error);
      Assert.AreEqual(expectedRotation.W, actualRotation.W, error);
    }
  }
}