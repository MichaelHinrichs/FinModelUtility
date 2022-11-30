using fin.util.asserts;
using fin.util.strings;
using NUnit.Framework;

namespace fin.language.writer {
  public class NestedLanguageWriterImplTests {
    [Test]
    public void TestAll() {
      var writer = new NestedStringBuilderImpl("  ");

      writer.Write("foo")
            .WriteLine("bar")
            .Write("(")
            .Nest("foo")
            .Nest("bar")
            .WriteLine(")")
            .Write("(")
            .Nest(
                w => w
                     .WriteLine("boo")
                     .WriteLine("(")
                     .Nest("hoo")
                     .WriteLine(")"))
            .WriteLine(")")
            .WriteLine("hello")
            .Write("goodbye");
      var actualText = writer.ToString();

      var expectedLines = new[] {
          "foobar",
          "(",
          "  foo",
          "  bar",
          ")",
          "(",
          "  boo",
          "  (",
          "    hoo",
          "  )",
          ")",
          "hello",
          "goodbye",
      };
      var expectedText = StringUtil.Concat(expectedLines, "\n");

      Expect.AreEqual(expectedText, actualText);
    }
  }
}