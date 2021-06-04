using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using fin.util.asserts;

namespace fin.data {
  [TestClass]
  public class AsyncCollectorTest {
    [TestMethod]
    public async Task TestToArray() {
      var collector = new AsyncCollector<string>();

      collector.Add("foo");
      collector.Add("bar");
      collector.Add(Task.FromResult("bar"));

      Expect.AreEqual(new[] {"foo", "bar", "awaited"},
                      await collector.ToArray());
    }
  }
}