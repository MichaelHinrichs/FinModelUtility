using NUnit.Framework;

namespace UoT.displaylist {
  public class DisplayListInstructionTest {
    [Test]
    public void TestNewEmpty() {
      var instruction = new DLCommand();

      Assert.AreEqual(0, instruction.Opcode);
      Assert.AreEqual(0, instruction.Low);
      Assert.AreEqual(0, instruction.High);

      foreach (var param in instruction.CMDParams) {
        Assert.AreEqual(0, param);
      }
    }

    [Test]
    public void TestNewOpcode() {
      var instruction = new DLCommand(0x12);

      Assert.AreEqual(0x12, instruction.Opcode);
      Assert.AreEqual(0x00000000, instruction.Low);
      Assert.AreEqual(0, instruction.High);

      Assert.AreEqual(0X12, instruction.Opcode);
      for (var i = 1; i < 8; ++i) {
        Assert.AreEqual(0, instruction.CMDParams[i]);
      }
    }

    [Test]
    public void TestNewFromSrc() {
      var src = new byte[]
          {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09};
      var instruction = new DLCommand(src, 1);

      Assert.AreEqual(0x01, instruction.Opcode);
      Assert.AreEqual(0x00020304, instruction.Low);
      Assert.AreEqual(0x05060708, instruction.High);

      for (var i = 0; i < 8; ++i) {
        Assert.AreEqual(i+1, instruction.CMDParams[i]);
      }
    }

    [Test]
    public void TestNewLowHigh() {
      var instruction = new DLCommand(0x01020304, 0x05060708);

      Assert.AreEqual(0x01, instruction.Opcode);
      Assert.AreEqual(0x00020304, instruction.Low);
      Assert.AreEqual(0x05060708, instruction.High);

      for (var i = 0; i < 8; ++i) {
        Assert.AreEqual(i + 1, instruction.CMDParams[i]);
      }
    }

    [Test]
    public void TestNewOpcodeLowHigh() {
      var instruction = new DLCommand(0x01, 0xff020304, 0x05060708);

      Assert.AreEqual(0x01, instruction.Opcode);
      Assert.AreEqual(0x00020304, instruction.Low);
      Assert.AreEqual(0x05060708, instruction.High);

      for (var i = 0; i < 8; ++i) {
        Assert.AreEqual(i + 1, instruction.CMDParams[i]);
      }
    }
  }
}