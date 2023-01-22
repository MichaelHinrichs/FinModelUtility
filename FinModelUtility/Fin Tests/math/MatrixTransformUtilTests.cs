using fin.math.matrix;
using fin.model.impl;

using NUnit.Framework;


namespace fin.math {
  public class MatrixTransformUtilTests {
    [Test]
    public void TestTranslation() {
      var expectedTranslation =
          new ModelImpl.PositionImpl { X = 2, Y = 3, Z = 4 };

      var matrix = MatrixTransformUtil.FromTranslation(
          expectedTranslation);

      var actualTranslation = new ModelImpl.PositionImpl();
      matrix.CopyTranslationInto(actualTranslation);

      Assert.AreEqual(expectedTranslation, actualTranslation);
    }

    [Test]
    public void TestRotation() {
      var expectedRotation = QuaternionUtil.Create(1.2f, 2.3f, 3.4f);

      var matrix = MatrixTransformUtil.FromRotation(
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
      var expectedScale = new ModelImpl.ScaleImpl { X = 3, Y = 4, Z = 5 };

      var matrix = MatrixTransformUtil.FromScale(
          expectedScale);

      var actualScale = new ModelImpl.ScaleImpl();
      matrix.CopyScaleInto(actualScale);

      Assert.AreEqual(expectedScale, actualScale);
    }


    [Test]
    public void TestTrs() {
      var expectedTranslation =
          new ModelImpl.PositionImpl { X = 2, Y = 3, Z = 4 };
      var expectedRotation = QuaternionUtil.Create(1.2f, 2.3f, 3.4f);
      var expectedScale = new ModelImpl.ScaleImpl { X = 3, Y = 4, Z = 5 };

      var trs = MatrixTransformUtil.FromTrs(
          expectedTranslation,
          expectedRotation,
          expectedScale);

      var actualTranslation = new ModelImpl.PositionImpl();
      trs.CopyTranslationInto(actualTranslation);

      trs.CopyRotationInto(out var actualRotation);

      var actualScale = new ModelImpl.ScaleImpl();
      trs.CopyScaleInto(actualScale);

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