using NUnit.Framework;

using schema.binary.util;


namespace System.IO {
  internal class FinTextReaderTests {
    [Test]
    public void TestGetPosition() {
      using var tw = CreateTextReader_("abc");

      Assert.AreEqual(0, tw.Position);
      Assert.AreEqual('a', tw.ReadChar());
      Assert.AreEqual(1, tw.Position);
    }

    [Test]
    public void TestSetPosition() {
      using var tw = CreateTextReader_("abc");

      tw.Position = 1;
      Assert.AreEqual('b', tw.ReadChar());
    }

    [Test]
    public void TestLength() {
      using var tw = CreateTextReader_("abc");
      Assert.AreEqual(3, tw.Length);
    }

    [Test]
    public void TestReadChar() {
      using var tw = CreateTextReader_("abc");

      Assert.AreEqual('a', tw.ReadChar());
      Assert.AreEqual('b', tw.ReadChar());
      Assert.AreEqual('c', tw.ReadChar());
    }

    [Test]
    public void TestAssertChar() {
      using var tw = CreateTextReader_("abc");

      tw.AssertChar('a');
      tw.AssertChar('b');
      tw.AssertChar('c');
    }

    [Test]
    public void TestReadUpTo() {
      using var tw = CreateTextReader_("abc,xyz, 123");

      Assert.AreEqual("abc", tw.ReadUpTo(","));
      Assert.AreEqual("xyz", tw.ReadUpTo(",", " "));
      Assert.AreEqual("123", tw.ReadUpTo(","));
    }

    [Test]
    public void TestReadWhile() {
      using var tw = CreateTextReader_("0001111");

      Assert.AreEqual(String.Empty, tw.ReadWhile("a"));
      Assert.AreEqual("000", tw.ReadWhile("0"));
      Assert.AreEqual("1111", tw.ReadWhile("1"));
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("255", 255)]
    public void TestReadByte(string inputText, byte expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadByte());
    }

    [Test]
    [TestCase("0x00", 0)]
    [TestCase("0xFF", 255)]
    [TestCase("0xff", 255)]
    [TestCase("0Xff", 255)]
    [TestCase("ff", 255)]
    public void TestReadHexByte(string inputText, byte expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadHexByte());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("127", 127)]
    [TestCase("-128", -128)]
    public void TestReadSByte(string inputText, sbyte expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadSByte());
    }

    [Test]
    [TestCase("0x00", 0)]
    [TestCase("0xFF", -1)]
    public void TestReadHexSByte(string inputText, sbyte expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadHexSByte());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("12345", 12345)]
    public void TestReadInt16(string inputText, short expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadInt16());
    }

    [Test]
    [TestCase("0", (ushort) 0)]
    [TestCase("12345", (ushort) 12345)]
    public void TestReadUInt16(string inputText, ushort expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadUInt16());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("1234567", 1234567)]
    public void TestReadInt32(string inputText, int expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadInt32());
    }

    [Test]
    [TestCase("0", (uint) 0)]
    [TestCase("1234567", (uint) 1234567)]
    public void TestReadUInt32(string inputText, uint expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadUInt32());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("123456789", 123456789)]
    public void TestReadInt64(string inputText, long expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadInt64());
    }

    [Test]
    [TestCase("0", (ulong) 0)]
    [TestCase("123456789", (ulong) 123456789)]
    public void TestReadUInt64(string inputText, ulong expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadUInt64());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("1", 1)]
    [TestCase("0.01", 0.01f)]
    [TestCase("-0.01", -0.01f)]
    public void TestReadSingle(string inputText, float expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadSingle());
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("1", 1)]
    [TestCase("0.01", 0.01)]
    [TestCase("-0.01", -0.01)]
    public void TestReadDouble(string inputText, double expectedValue) {
      using var tw = CreateTextReader_(inputText);
      Asserts.Equal(expectedValue, tw.ReadDouble());
    }


    private static FinTextReader CreateTextReader_(string text) {
      var ms = new MemoryStream();

      var sw = new StreamWriter(ms);
      sw.Write(text);
      sw.Flush();
      ms.Position = 0;

      return new FinTextReader(ms);
    }
  }
}