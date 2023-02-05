using NUnit.Framework;


namespace System.IO {
  internal class FinTextReaderTests {
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
    public void TestGetPosition() {
      using var tw = CreateTextReader_("abc");

      Assert.AreEqual(0, tw.Position);
      Assert.AreEqual('a', tw.ReadChar());
      Assert.AreEqual(1, tw.Position);
    }

    [Test]
    public void TestSetPosition() {
      using var tw = CreateTextReader_("abc");

      Assert.AreEqual(1, tw.Position);
      Assert.AreEqual('b', tw.ReadChar());
    }

    private static FinTextReader CreateTextReader_(string text) {
      var ms = new MemoryStream();

      var sw = new StreamWriter(ms);
      sw.Write(text);
      ms.Position = 0;

      return new FinTextReader(ms);
    }
  }
}