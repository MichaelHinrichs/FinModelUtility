using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class ModelViewMatrixTransformerTest {
    [TestMethod]
    public void TestTranslate() {
      var t = new SoftwareModelViewMatrixTransformer();
      t.Translate(1, 2, 3);

      var x = .12;
      var y = .34;
      var z = .56;

      t.ProjectVertex(ref x, ref y, ref z);

      Assert.AreEqual((1.12, 2.34, 3.56), (x, y, z));
    }

    [TestMethod]
    public void TestSetMatrixTranslate() {
      var m = Matrix4x4.CreateTranslation(1, 2, 3);

      var t = new SoftwareModelViewMatrixTransformer();
      t.Set(m);

      var x = .12;
      var y = .34;
      var z = .56;

      t.ProjectVertex(ref x, ref y, ref z);

      Assert.AreEqual((1.12, 2.34, 3.56), (x, y, z));
    }
  }
}
