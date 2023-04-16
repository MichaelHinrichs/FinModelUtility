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

    [Test]
    [TestCase((ushort) 0, 0)]
    [TestCase((ushort) 0x1, 0.0000152587890625)]
    [TestCase((ushort) 0x0080, 0.001953125)]
    [TestCase((ushort) 0x8000, .5)]
    [TestCase((ushort) 0xFFFF, 0.9999847412109375)]
    public void TestConvertBinaryFractionToFloat(ushort inputBinaryFraction,
                                                 double expected)
      => Assert.AreEqual(expected,
                         BitLogic.ConvertBinaryFractionToDouble(
                             inputBinaryFraction), .00000001);
  }
}