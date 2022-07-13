using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.gl {
  [TestClass]
  public class GlUtilTest {
    [TestMethod]
    public void CrossProduct() {
      GlUtil.CrossProduct3(
          4, 5, 6,
          11, 13, 17,
          out var x, out var y, out var z);
      
      Assert.AreEqual(x, 7);
      Assert.AreEqual(y, -2);
      Assert.AreEqual(z, -3);
    }

    [TestMethod]
    public void Normalizing() {
      var x = 5d;
      var y = 6d;
      var z = 7d;

      GlUtil.Normalize3(ref x, ref y, ref z);

      Assert.AreEqual(x, .4767, .001);
      Assert.AreEqual(y, .5721, .001);
      Assert.AreEqual(z, .6674, .001);
    }

  }
}