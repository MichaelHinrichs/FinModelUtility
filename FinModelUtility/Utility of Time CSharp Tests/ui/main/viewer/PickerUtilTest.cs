using NUnit.Framework;

namespace UoT.ui.main.viewer {
  public class PickerUtilTest {
    [SetUp]
    public void SetUp() {
      PickerUtil.Reset();
    }

    [Test]
    public void EmitsExpectedRs() {
      for (var r = 0; r <= 255; ++r) {
        PickerUtil.NextRgb(out var nextR, out var nextG, out var nextB);

        Assert.AreEqual(r, nextR, "Different R");
        Assert.AreEqual(0, nextG, "Different G");
        Assert.AreEqual(0, nextB, "Different B");
      }
    }

    [Test]
    public void EmitsExpectedGs() {
      PickerUtil.NextRgb(out var nextR, out var nextG, out var nextB);
      for (var g = 0; g <= 255; ++g) {
        Assert.AreEqual((0, g, 0), (nextR, nextG, nextB));

        for (var r = 0; r <= 255; ++r) {
          PickerUtil.NextRgb(out nextR, out nextG, out nextB);
        }
      }
    }

    [Test]
    public void EmitsExpectedBs() {
      PickerUtil.NextRgb(out var nextR, out var nextG, out var nextB);
      for (var b = 0; b <= 255; ++b) {
        Assert.AreEqual((0, 0, b), (nextR, nextG, nextB));

        for (var g = 0; g <= 255; ++g) {
          for (var r = 0; r <= 255; ++r) {
            PickerUtil.NextRgb(out nextR, out nextG, out nextB);
          }
        }
      }
    }
  }
}