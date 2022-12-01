using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using schema.util;
using System.Collections.Generic;
using System.Text;


namespace schema.io {
  public class NestedListsTest {
    [Test]
    public void TestEmptyList() {
      var impl = new NestedList<string>();
      AssertSequence_(new string[] { }, impl);
    }

    [Test]
    public void TestEmptyNestedLists() {
      var impl = new NestedList<string>();
      impl.Enter().Enter().Enter();
      AssertSequence_(new string[] { }, impl);
    }

    [Test]
    public async Task TestValues() {
      var impl = new NestedList<string>();

      impl.Add("first");
      impl.Add("second");
      impl.Add("third");

      AssertSequence_(new[] {"first", "second", "third"}, impl);
    }

    [Test]
    public async Task TestNestedValues() {
      var impl = new NestedList<string>();
      
      impl.Add("before child");
      var child = impl.Enter();
      impl.Add("after child");
      
      child.Add("in child");

      AssertSequence_(new[] { "before child", "in child", "after child" }, impl);
    }

    private void AssertSequence_<T>(
        IEnumerable<T> enumerableA,
        IEnumerable<T> enumerableB) {
      var enumeratorA = enumerableA.GetEnumerator();
      var enumeratorB = enumerableB.GetEnumerator();

      var hasA = enumeratorA.MoveNext();
      var hasB = enumeratorB.MoveNext();

      var index = 0;
      while (hasA && hasB) {
        var currentA = enumeratorA.Current;
        var currentB = enumeratorB.Current;

        if (!object.Equals(currentA, currentB)) {
          Asserts.Fail(
              $"Expected {currentA} to equal {currentB} at index ${index}.");
        }
        index++;

        hasA = enumeratorA.MoveNext();
        hasB = enumeratorB.MoveNext();
      }

      Asserts.True(!hasA && !hasB,
                   "Expected enumerables to be equal:\n" +
                   $"  A: {ConvertSequenceToString_(enumerableA)}\n" +
                   $"  B: {ConvertSequenceToString_(enumerableB)}");
    }

    private string ConvertSequenceToString_(IEnumerable enumerable) {
      var str = new StringBuilder();
      foreach (var value in enumerable) {
        if (str.Length > 0) {
          str.Append(", ");
        }
        str.Append(value);
      }
      return str.ToString();
    }
  }
}