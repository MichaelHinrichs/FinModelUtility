using fin.math.matrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class ModelViewMatrixTransformerTests {
    [TestMethod]
    public void TestTranslate() {
      var t = new SoftwareModelViewMatrixTransformer();
      t.Translate(1, 2, 3);

      var x = .12f;
      var y = .34f;
      var z = .56f;

      t.ProjectVertex(ref x, ref y, ref z);

      Assert.AreEqual((1.12f, 2.34f, 3.56f), (x, y, z));
    }

    [TestMethod]
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
