using NUnit.Framework;


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
      using var tw = CreateTextReader_("abc,123");

      Assert.AreEqual(String.Empty, tw.ReadUpTo("a"));
      Assert.AreEqual("abc", tw.ReadUpTo(","));
      tw.AssertString(",");
      Assert.AreEqual("123", tw.ReadUpTo(","));
    }

    [Test]
    public void TestReadWhile() {
      using var tw = CreateTextReader_("0001111");

      Assert.AreEqual(String.Empty, tw.ReadWhile("a"));
      Assert.AreEqual("000", tw.ReadWhile("0"));
      Assert.AreEqual("1111", tw.ReadWhile("1"));
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