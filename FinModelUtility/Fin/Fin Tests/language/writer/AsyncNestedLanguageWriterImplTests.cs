using System.Threading.Tasks;

using fin.util.asserts;
using fin.util.strings;

using NUnit.Framework;

namespace fin.language.writer {
  public class AsyncNestedLanguageWriterImplTests {
    [Test]
    public async Task TestAll() {
      var writer = new AsyncNestedStringBuilderImpl("  ");

      writer.Write("foo")
            .WriteLine(Task.FromResult("bar"))
            .Write("(")
            .Nest("foo")
            .Nest(Task.FromResult("bar"))
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
            .Write(Task.FromResult("goodbye"));
      var actualText = await writer.ToString();

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