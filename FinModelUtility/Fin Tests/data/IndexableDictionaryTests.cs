using System.Collections;
using System.Linq;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.data {
  [TestClass]
  public class IndexableDictionaryTests {
    private class IndexableImpl : IIndexable {
      public IndexableImpl(int index) {
        this.Index = index;
      }

      public int Index { get; }
    }

    [TestMethod]
    public void TestEnumeratorClear() {
      var dict = new IndexableDictionary<IndexableImpl, string>();

      var index1 = new IndexableImpl(1);
      
      Assert.AreEqual(false, dict.TryGetValue(index1, out _));

      dict[index1] = "string1";

      Assert.AreEqual(true, dict.TryGetValue(index1, out _));

      dict.Clear();

      Assert.AreEqual(false, dict.TryGetValue(index1, out _));
    }

    [TestMethod]
    public void TestEnumeratorGet() {
      var dict = new IndexableDictionary<IndexableImpl, string>();

      var index1 = new IndexableImpl(1);
      var index3 = new IndexableImpl(3);
      var index5 = new IndexableImpl(5);

      dict[index3] = "string3";
      dict[index5] = "string5";
      dict[index1] = "string1";

      Assert.AreEqual("string1", dict[index1]);
      Assert.AreEqual("string3", dict[index3]);
      Assert.AreEqual("string5", dict[index5]);
    }

    [TestMethod]
    public void TestEnumeratorTryGet() {
      var dict = new IndexableDictionary<IndexableImpl, string>();

      var index1 = new IndexableImpl(1);
      var index3 = new IndexableImpl(3);
      var index5 = new IndexableImpl(5);

      Assert.AreEqual(false, dict.TryGetValue(index1, out _));
      Assert.AreEqual(false, dict.TryGetValue(index3, out _));
      Assert.AreEqual(false, dict.TryGetValue(index5, out _));

      dict[index3] = "string3";
      dict[index5] = "string5";
      dict[index1] = "string1";

      Assert.AreEqual(true, dict.TryGetValue(index1, out var value1));
      Assert.AreEqual("string1", value1);

      Assert.AreEqual(true, dict.TryGetValue(index3, out var value3));
      Assert.AreEqual("string3", value3);

      Assert.AreEqual(true, dict.TryGetValue(index5, out var value5));
      Assert.AreEqual("string5", value5);
    }

    [TestMethod]
    public void TestEnumeratorLinq() {
      var dict = new IndexableDictionary<IndexableImpl, string>();

      var index1 = new IndexableImpl(1);
      var index3 = new IndexableImpl(3);
      var index5 = new IndexableImpl(5);

      dict[index3] = "string3";
      dict[index5] = "string5";
      dict[index1] = "string1";

      var values = dict.ToArray();
      Asserts.Equal(
          new[] {
              (index1, "string1"),
              (index3, "string3"),
              (index5, "string5")
          }, values);
    }

    [TestMethod]
    public void TestEnumeratorManually() {
      var dict = new IndexableDictionary<IndexableImpl, string>();

      var index1 = new IndexableImpl(1);
      var index3 = new IndexableImpl(3);
      var index5 = new IndexableImpl(5);

      dict[index3] = "string3";
      dict[index5] = "string5";
      dict[index1] = "string1";

      var enumerator = ((IEnumerable) dict).GetEnumerator();

      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual((index1, "string1"), enumerator.Current);

      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual((index3, "string3"), enumerator.Current);

      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual((index5, "string5"), enumerator.Current);

      Assert.AreEqual(false, enumerator.MoveNext());
    }
  }
}