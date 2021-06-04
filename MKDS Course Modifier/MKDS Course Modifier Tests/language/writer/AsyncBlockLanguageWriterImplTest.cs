using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using fin.util.asserts;
using fin.util.strings;

namespace fin.language.writer {
  [TestClass]
  public class AsyncBlockLanguageWriterImplTest {
    [TestMethod]
    public async Task TestAll() {
      var writer = new AsyncBlockStringBuilderImpl();

      writer.Write("foo")
            .Write(Task.FromResult("bar\n"))
            .WriteBlock("{", "foo", "}\n")
            .WriteBlock("[", Task.FromResult("bar"), "]\n")
            .WriteBlock("(", w => w.Write("boo"), ")");

      Expect.AreEqual(
          StringUtil.Concat(new[] {"foobar", "{foo}", "[bar]", "(boo)"}, "\n"),
          await writer.ToString());
    }
  }
}