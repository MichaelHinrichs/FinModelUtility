using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class BitLogicTests {
    [TestMethod]
    public void ExtractFromRight() {
      Assert.AreEqual((uint) 0b1111,
                      BitLogic.ExtractFromRight(0b00001111, 0, 4));
      Assert.AreEqual((uint) 0b1111,
                      BitLogic.ExtractFromRight(0b11110000, 4, 4));
      Assert.AreEqual((uint) 0b1011, BitLogic.ExtractFromRight(0b101100, 2, 4));
    }
  }
}