using fin.math.matrix;
using NUnit.Framework;
using System.Numerics;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class ModelViewMatrixTransformerTests {
    [Test]
    public void TestTranslate() {
      var t = new SoftwareModelViewMatrixTransformer();
      t.Translate(1, 2, 3);

      var xyz = new Vector3(.12f, .34f, .56f);

      t.ProjectVertex(ref xyz);

      Assert.AreEqual(new Vector3(1.12f, 2.34f, 3.56f), xyz);
    }

    [Test]
    public void TestSetMatrixTranslate() {
      var m = MatrixTransformUtil.FromTranslation(1, 2, 3);

      var t = new SoftwareModelViewMatrixTransformer();
      t.Set(m);

      var xyz = new Vector3(.12f, .34f, .56f);

      t.ProjectVertex(ref xyz);

      Assert.AreEqual(new Vector3(1.12f, 2.34f, 3.56f), xyz);
    }
  }
}
