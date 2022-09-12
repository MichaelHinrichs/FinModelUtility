using System.Collections;
using System.Collections.Generic;
using System.Linq;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.data.queue {
  [TestClass]
  public class FinQueueTest {
    [TestMethod]
    public void TestEnqueueSeparately() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);

      queue.Enqueue("value1");
      Assert.AreEqual(1, queue.Count);

      queue.Enqueue("value2");
      Assert.AreEqual(2, queue.Count);

      Assert.AreEqual("value1", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("value2", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public void TestEnqueueParams() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);

      queue.Enqueue("value1", "value2");
      Assert.AreEqual(2, queue.Count);

      queue.Enqueue("value3", "value4");
      Assert.AreEqual(4, queue.Count);

      Assert.AreEqual("value1", queue.Dequeue());
      Assert.AreEqual(3, queue.Count);
      Assert.AreEqual("value2", queue.Dequeue());
      Assert.AreEqual(2, queue.Count);
      Assert.AreEqual("value3", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("value4", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public void TestEnqueueEnumerables() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);

      queue.Enqueue(new[] {"value1", "value2"});
      Assert.AreEqual(2, queue.Count);

      queue.Enqueue(new[] {"value3", "value4"});
      Assert.AreEqual(4, queue.Count);

      Assert.AreEqual("value1", queue.Dequeue());
      Assert.AreEqual(3, queue.Count);
      Assert.AreEqual("value2", queue.Dequeue());
      Assert.AreEqual(2, queue.Count);
      Assert.AreEqual("value3", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("value4", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public void TestConstructorParams() {
      var queue = new FinQueue<string>("value1", "value2");
      Assert.AreEqual(2, queue.Count);

      Assert.AreEqual("value1", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("value2", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public void TestConstructorEnumerable() {
      var queue = new FinQueue<string>(new[] {"value1", "value2"});
      Assert.AreEqual(2, queue.Count);

      Assert.AreEqual("value1", queue.Dequeue());
      Assert.AreEqual(1, queue.Count);
      Assert.AreEqual("value2", queue.Dequeue());
      Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public void TestEnumeratorLinq() {
      var expectedValues = new[] {"value1", "value2", "value3"};
      var queue = new FinQueue<string>(expectedValues);

      var actualValues = queue.ToArray();
      Asserts.Equal(expectedValues, actualValues);
    }

    [TestMethod]
    public void TestEnumeratorManually() {
      var queue = new FinQueue<string>("value1", "value2", "value3");

      var enumerator = ((IEnumerable) queue).GetEnumerator();

      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual("value1", enumerator.Current);
      
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual("value2", enumerator.Current);
      
      Assert.AreEqual(true, enumerator.MoveNext());
      Assert.AreEqual("value3", enumerator.Current);
 
      Assert.AreEqual(false, enumerator.MoveNext());
    }

    [TestMethod]
    public void TestClear() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);

      queue.Enqueue(new[] {"value1", "value2", "value3"});
      Assert.AreEqual(3, queue.Count);

      queue.Clear();
      Assert.AreEqual(0, queue.Count);

      Assert.AreEqual(false, queue.TryPeek(out _));
      Assert.AreEqual(false, queue.TryDequeue(out _));
    }

    [TestMethod]
    public void TestTryDequeue() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);
      Assert.AreEqual(false, queue.TryDequeue(out _));

      var values = new[] {"value1", "value2", "value3"};

      queue.Enqueue(values);
      Assert.AreEqual(3, queue.Count);

      foreach (var expectedValue in values) {
        Assert.AreEqual(true, queue.TryDequeue(out var actualValue));
        Assert.AreEqual(expectedValue, actualValue);
      }

      Assert.AreEqual(false, queue.TryDequeue(out _));
    }

    [TestMethod]
    public void TestTryPeek() {
      var queue = new FinQueue<string>();
      Assert.AreEqual(0, queue.Count);
      Assert.AreEqual(false, queue.TryPeek(out _));

      var values = new[] {"value1", "value2", "value3"};

      queue.Enqueue(values);
      Assert.AreEqual(3, queue.Count);

      foreach (var expectedValue in values) {
        Assert.AreEqual(true, queue.TryPeek(out var actualValue));
        Assert.AreEqual(expectedValue, actualValue);

        queue.Dequeue();
      }

      Assert.AreEqual(false, queue.TryPeek(out _));
    }

    [TestMethod]
    public void TestPeek() {
      var expectedValues = new[] { "value1", "value2", "value3" };
      var queue = new FinQueue<string>(expectedValues);

      foreach (var expectedValue in expectedValues) {
        Assert.AreEqual(expectedValue, queue.Peek());

        queue.Dequeue();
      }
    }
  }
}