using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace fin.math {
  public class BitLogicTests {
    [Test]
    public void ExtractFromRight() {
      Assert.AreEqual((uint) 0b1111,
                      BitLogic.ExtractFromRight(0b00001111, 0, 4));
      Assert.AreEqual((uint) 0b1111,
                      BitLogic.ExtractFromRight(0b11110000, 4, 4));
      Assert.AreEqual((uint) 0b1011, BitLogic.ExtractFromRight(0b101100, 2, 4));
    }

    [Test]
    [TestCase(1, 0, true)]
    [TestCase(2, 0, false)]
    [TestCase(2, 1, true)]
    public void TestGetBitInt(int value, int bit, bool expectedValue)
      => Assert.AreEqual(expectedValue, value.GetBit(bit));

    [Test]
    [TestCase(0, 0)]
    [TestCase(1, 0b1)]
    [TestCase(2, 0b11)]
    [TestCase(3, 0b111)]
    [TestCase(4, 0b1111)]
    public void TestMask(int numBits, int expectedMask)
      => Assert.AreEqual(expectedMask, (int) BitLogic.GetMask(numBits));
  }
}