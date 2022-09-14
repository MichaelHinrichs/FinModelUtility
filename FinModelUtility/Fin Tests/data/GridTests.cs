using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.data {
  [TestClass]
  public class GridTests {
    [TestMethod]
    public void TestDefaultValue() {
      var impl = new Grid<string>(3, 3, "foobar");

      Assert.AreEqual(3, impl.Width);
      Assert.AreEqual(3, impl.Height);

      for (var y = 0; y < impl.Height; ++y) {
        for (var x = 0; x < impl.Width; ++x) {
          Assert.AreEqual("foobar", impl[x, y]);
        }
      }
    }

    [TestMethod]
    public void TestDefaultHandler() {
      var index = 0;
      var impl = new Grid<int>(3, 3, () => index++);

      Assert.AreEqual(3, impl.Width);
      Assert.AreEqual(3, impl.Height);

      for (var y = 0; y < impl.Height; ++y) {
        for (var x = 0; x < impl.Width; ++x) {
          Assert.AreEqual(y * impl.Width + x, impl[x, y]);
        }
      }
    }

    [TestMethod]
    public void TestSetValues() {
      var impl = new Grid<string>(3, 3);

      Assert.AreEqual(3, impl.Width);
      Assert.AreEqual(3, impl.Height);

      for (var y = 0; y < impl.Height; ++y) {
        for (var x = 0; x < impl.Width; ++x) {
          impl[x, y] = $"({x}, {y})";
        }
      }

      for (var y = 0; y < impl.Height; ++y) {
        for (var x = 0; x < impl.Width; ++x) {
          Assert.AreEqual($"({x}, {y})", impl[x, y]);
        }
      }
    }

    [TestMethod]
    public void TestFailsOutside() {
      var impl = new Grid<string>(3, 3);

      Assert.ThrowsException<AssertionException>(
          () => { impl[-1, 0] = "value"; },
          "\"Expected (-1, 0) to be a valid index in grid of size (3, 3).\"");
      Assert.ThrowsException<AssertionException>(
          () => { impl[0, -1] = "value"; },
          "\"Expected (-1, 0) to be a valid index in grid of size (3, 3).\"");
      Assert.ThrowsException<AssertionException>(
          () => { impl[4, 0] = "value"; },
          "\"Expected (4, 0) to be a valid index in grid of size (3, 3).\"");
      Assert.ThrowsException<AssertionException>(
          () => { impl[0, 4] = "value"; },
          "\"Expected (0, 4) to be a valid index in grid of size (3, 3).\"");
    }
  }
}