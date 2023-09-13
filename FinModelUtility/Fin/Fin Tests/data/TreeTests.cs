using System.Collections.Generic;

using fin.data.queue;
using fin.util.asserts;

using NUnit.Framework;

namespace fin.data {
  public class TreeTests {
    [Test]
    public void TestIterating() {
      var nodeRoot = new Node<string> { Value = "root" };

      var nodeFoo = new Node<string> { Value = "foo" };
      nodeRoot.AddChild(nodeFoo);
      var nodeBar = new Node<string> { Value = "bar" };
      nodeRoot.AddChild(nodeBar);

      var node123 = new Node<string> { Value = "123" };
      nodeFoo.AddChild(node123);

      var nodeAbc = new Node<string> { Value = "abc" };
      nodeBar.AddChild(nodeAbc);

      var actualValues = new List<string>();
      var finQueue = new FinQueue<Node<string>>(nodeRoot);
      while (finQueue.TryDequeue(out var node)) {
        actualValues.Add(node.Value);
        finQueue.Enqueue(node.Children);
      }

      Asserts.Equal<IEnumerable<string>>(
          actualValues,
          new[] { "root", "foo", "bar", "123", "abc" });
    }

    [Test]
    public void TestAncestors() {
      var nodeRoot = new Node<string> { Value = "root" };

      var nodeFoo = new Node<string> { Value = "foo" };
      nodeRoot.AddChild(nodeFoo);

      var node123 = new Node<string> { Value = "123" };
      nodeFoo.AddChild(node123);

      var actualValues = new List<string>();
      var finQueue = new FinQueue<Node<string>>(node123);
      while (finQueue.TryDequeue(out var node)) {
        actualValues.Add(node.Value);

        if (node.Parent != null) {
          finQueue.Enqueue(node.Parent);
        }
      }

      Asserts.Equal<IEnumerable<string>>(
          actualValues,
          new[] { "123", "foo", "root" });
    }

    [Test]
    public void TestReplacingParent() {
      var nodeFoo = new Node<string> { Value = "foo" };
      var nodeBar = new Node<string> { Value = "foo" };

      Assert.AreEqual(0, nodeFoo.Children.Count);
      Assert.AreEqual(0, nodeBar.Children.Count);

      var nodeChild = new Node<string> { Value = "child" };
      Assert.Null(nodeChild.Parent);

      nodeChild.Parent = nodeFoo;
      Assert.AreEqual(nodeFoo, nodeChild.Parent);
      Asserts.Equal(nodeFoo.Children, new[] { nodeChild });
      Assert.AreEqual(0, nodeBar.Children.Count);

      nodeBar.AddChild(nodeChild);
      Assert.AreEqual(nodeBar, nodeChild.Parent);
      Asserts.Equal(nodeBar.Children, new[] { nodeChild });
      Assert.AreEqual(0, nodeFoo.Children.Count);

      nodeChild.Parent = nodeFoo;
      Assert.AreEqual(nodeFoo, nodeChild.Parent);
      Asserts.Equal(nodeFoo.Children, new[] { nodeChild });
      Assert.AreEqual(0, nodeBar.Children.Count);
    }
  }
}