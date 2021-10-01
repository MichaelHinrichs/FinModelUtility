using System.Numerics;

using fin.math.matrix;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.math {
  [TestClass]
  public class BitLogicTest {
    [TestMethod]
    public void ExtractFromRight() {
      Assert.AreEqual(0b1111, BitLogic.ExtractFromRight(0b00001111, 0, 4));
      Assert.AreEqual(0b1111, BitLogic.ExtractFromRight(0b11110000, 4, 4));
      Assert.AreEqual(0b1011, BitLogic.ExtractFromRight(0b101100, 2, 4));
    }

    [TestMethod]
    /*public void ExtractFromLeft() {
      Assert.AreEqual(0b1111, BitLogic.ExtractFromLeft(0b11110000, 0, 4));
      Assert.AreEqual(0b1111, BitLogic.ExtractFromLeft(0b00001111, 4, 4));
      Assert.AreEqual(0b1011, BitLogic.ExtractFromLeft(0b001011, 2, 4));
    }*/
  }
}