using fin.math.matrix;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class ModelViewMatrixTransformerTests {
    [Test]
    public void TestTranslate() {
      var t = new SoftwareModelViewMatrixTransformer();
      t.Translate(1, 2, 3);

      var x = .12f;
      var y = .34f;
      var z = .56f;

      t.ProjectVertex(ref x, ref y, ref z);

      Assert.AreEqual((1.12f, 2.34f, 3.56f), (x, y, z));
    }

    [Test]
    public void TestSetMatrixTranslate() {
      var m = MatrixTransformUtil.FromTranslation(1, 2, 3);

      var t = new SoftwareModelViewMatrixTransformer();
      t.Set(m);

      var x = .12f;
      var y = .34f;
      var z = .56f;

      t.ProjectVertex(ref x, ref y, ref z);

      Assert.AreEqual((1.12f, 2.34f, 3.56f), (x, y, z));
    }
  }
}
