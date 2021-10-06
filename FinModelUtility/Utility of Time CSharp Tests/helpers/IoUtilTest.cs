using NUnit.Framework;

namespace UoT.helpers {
  public class IoUtilTest {
    [Test]
    public void TestSplitAddress() {
      IoUtil.SplitAddress(0x12345678, out var bank, out var offset);

      Assert.AreEqual(0x12, bank);
      Assert.AreEqual(0x345678, offset);
    }

    [Test]
    public void TestMergeAddress() {
      Assert.AreEqual(0x12345678, IoUtil.MergeAddress(0x12, 0x345678));
    }


    [Test]
    public void TestReadUInt24() {
      var buffer = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
      Assert.AreEqual(66051, IoUtil.ReadUInt24(buffer, 1));
    }

    [Test]
    public void TestReadUInt32() {
      var buffer = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
      Assert.AreEqual(16909060, IoUtil.ReadUInt32(buffer, 1));
    }

    [Test]
    public void TestWriteInt16() {
      var buffer = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, };

      var offset = 1;
      IoUtil.WriteInt16(buffer, ref offset, 0x0987);

      Assert.AreEqual(buffer[0], 0x12);
      Assert.AreEqual(buffer[1], 0x09);
      Assert.AreEqual(buffer[2], 0x87);
      Assert.AreEqual(buffer[3], 0x78);
      Assert.AreEqual(buffer[4], 0x90);
    }

    [Test]
    public void TestWriteInt32() {
      var buffer = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, };

      var offset = 1;
      IoUtil.WriteInt32(buffer, 0x09876543, ref offset);

      Assert.AreEqual(buffer[0], 0x12);
      Assert.AreEqual(buffer[1], 0x09);
      Assert.AreEqual(buffer[2], 0x87);
      Assert.AreEqual(buffer[3], 0x65);
      Assert.AreEqual(buffer[4], 0x43);
    }
  }
}