using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using fin.util.asserts;

namespace fin.data {
  [TestClass]
  public class AsyncCollectorTest {
    [TestMethod]
    public async Task TestToArray() {
      var collector = new AsyncCollector<string>();

      var delay = new TaskCompletionSource();

      collector.Add("foo");
      collector.Add("bar");
      collector.Add(delay.Task.ContinueWith(_ => "awaited"));

      var toArray = collector.ToArray();
      collector.Clear();

      delay.SetResult();

      Expect.AreArraysEqual(new[] {"foo", "bar", "awaited"},
                      await toArray);
    }
  }
}