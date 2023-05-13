using System.Collections.Generic;


namespace fin.data {
  public interface INode<out T, TSelf> where TSelf : INode<T, TSelf> {
    T Value { get; }

    TSelf? Parent { get; set; }

    IReadOnlyList<TSelf> Children { get; }
    void AddChild(TSelf child);
  }

  public sealed class Node<T> : INode<T, Node<T>> {
    private Node<T>? parent_;
    private readonly List<Node<T>> children_ = new();

    public T Value { get; set; }

    public Node<T>? Parent {
      get => this.parent_;
      set {
        this.parent_?.children_.Remove(this);

        this.parent_ = value;

        this.parent_?.children_.Add(this);
      }
    }

    public IReadOnlyList<Node<T>> Children => this.children_;

    public void AddChild(Node<T> child) {
      if (child.Parent == this) {
        return;
      }

      child.parent_?.children_.Remove(child);

      child.parent_ = this;
      this.children_.Add(child);
    }

    public override string ToString() => this.Value?.ToString() ?? "undefined";
  }
}