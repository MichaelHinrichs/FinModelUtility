using System;

using fin.util.strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.data {
  [TestClass]
  public class LazyDictionaryTest {
    [TestMethod]
    public void TestWithKeyAndValueHandler() {
      var invokeCount = 0;
      var lazyReverseMap = new LazyDictionary<string, string>(inStr => {
        invokeCount++;
        return inStr.Reverse();
      });

      Assert.AreEqual(0, lazyReverseMap.Count);
      Assert.AreEqual(0, invokeCount);

      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);

      // Reuses existing value
      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);
    }

    [TestMethod]
    public void TestWithDictionaryKeyAndValueHandler() {
      var invokeCount = 0;
      LazyDictionary<string, string>? lazyReverseMap = null;
      lazyReverseMap = new LazyDictionary<string, string>((dict, inStr) => {
        Assert.AreSame(lazyReverseMap, dict);
        invokeCount++;
        return inStr.Reverse();
      });

      Assert.AreEqual(0, lazyReverseMap.Count);
      Assert.AreEqual(0, invokeCount);

      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);

      // Reuses existing value
      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);
    }

    [TestMethod]
    public void TestSettingValuesDirectly() {
      var lazyReverseMap = new LazyDictionary<string, string>(
          _ => throw new NotImplementedException());

      Assert.AreEqual(0, lazyReverseMap.Count);

      lazyReverseMap["reverse"] = "esrever";
      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
    }

    [TestMethod]
    public void TestClear() {
      var invokeCount = 0;
      var lazyReverseMap = new LazyDictionary<string, string>(inStr => {
        invokeCount++;
        return inStr.Reverse();
      });

      Assert.AreEqual(0, lazyReverseMap.Count);
      Assert.AreEqual(0, invokeCount);

      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);

      lazyReverseMap.Clear();
      Assert.AreEqual(0, lazyReverseMap.Count);
      Assert.AreEqual(1, invokeCount);

      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(1, lazyReverseMap.Count);
      Assert.AreEqual(2, invokeCount);
    }

    [TestMethod]
    public void TestContainsKey() {
      var lazyReverseMap =
          new LazyDictionary<string, string>(inStr => inStr.Reverse());

      Assert.AreEqual(false, lazyReverseMap.ContainsKey("reverse"));

      Assert.AreEqual("esrever", lazyReverseMap["reverse"]);
      Assert.AreEqual(true, lazyReverseMap.ContainsKey("reverse"));

      lazyReverseMap.Clear();
      Assert.AreEqual(false, lazyReverseMap.ContainsKey("reverse"));
    }
  }
}