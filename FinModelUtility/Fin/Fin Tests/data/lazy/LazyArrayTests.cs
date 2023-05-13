using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace fin.data.lazy {
  public class LazyArrayTests {
    [Test]
    public void TestWithKeyAndValueHandler() {
      var invokeCount = 0;
      var lazyReverseMap = new LazyArray<string>(2, i => {
        invokeCount++;
        return $"foo{i}";
      });

      Assert.AreEqual(2, lazyReverseMap.Count);
      Assert.AreEqual(0, invokeCount);

      Assert.AreEqual("foo1", lazyReverseMap[1]);
      Assert.AreEqual(2, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);

      // Reuses existing value
      Assert.AreEqual("foo1", lazyReverseMap[1]);
      Assert.AreEqual(2, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);
    }
  }
}