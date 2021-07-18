using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UoT.ui.common.component {
  public class BetterTreeNode<T> where T : class {
    private readonly TreeNode? impl_;
    private readonly TreeNodeCollection collection_;

    public BetterTreeNode<T>? Parent { get; private set; }

    public int AbsoluteIndex { get; }

    private readonly IList<BetterTreeNode<T>> absoluteChildren_ =
        new List<BetterTreeNode<T>>();

    // TODO: Possible to remove unboxing?
    public T? AssociatedData { get; set; }
    public string? Text => this.impl_?.Text;

    public BetterTreeNode(TreeNode? impl, TreeNodeCollection collection) {
      this.impl_ = impl;

      if (impl != null) {
        impl.Tag = this;
      }

      this.collection_ = collection;

      this.AbsoluteIndex = impl?.Index ?? 0;
    }

    public BetterTreeNode<T> Add(string text) {
      var childTreeNode = this.collection_.Add(text);

      var childBetterTreeNode =
          new BetterTreeNode<T>(childTreeNode, childTreeNode.Nodes);
      childBetterTreeNode.Parent = this;

      this.absoluteChildren_.Add(childBetterTreeNode);
      return childBetterTreeNode;
    }

    public void Add(BetterTreeNode<T> node) => this.collection_.Add(node.impl_);

    public void ResetChildrenRecursively(
        Func<BetterTreeNode<T>, bool>? filter = null) {
      this.ClearRecursively_();
      this.ReaddChildrenRecursively(filter);
    }

    private void ClearRecursively_() {
      // TODO: Remove unboxing?
      foreach (var childTreeNodeObj in this.collection_) {
        var childTreeNode = (TreeNode) childTreeNodeObj;
        var childBetterTreeNode = (BetterTreeNode<T>) childTreeNode.Tag;

        childBetterTreeNode.ClearRecursively_();
      }

      this.collection_.Clear();
    }

    private void ReaddChildrenRecursively(
        Func<BetterTreeNode<T>, bool>? filter) {
      // TODO: Remove unboxing?
      foreach (var childBetterTreeNode in this.absoluteChildren_) {
        if (filter != null && !filter(childBetterTreeNode)) {
          continue;
        }

        this.Add(childBetterTreeNode);
        childBetterTreeNode.ReaddChildrenRecursively(filter);
      }
    }
  }

  public class BetterTreeView<T> where T : class {
    // TODO: Add tests.
    // TODO: Add support for different hierarchies.

    private readonly TreeView impl_;

    private BetterTreeViewComparer comparer_ =
        new BetterTreeViewComparer();

    public BetterTreeNode<T> Root { get; }

    public delegate void SelectedHandler(BetterTreeNode<T> betterTreeNode);

    public event SelectedHandler Selected = delegate {};

    public IComparer<BetterTreeNode<T>>? Comparer {
      get => this.comparer_.Comparer;
      set => this.comparer_.Comparer = value;
    }

    public BetterTreeView(TreeView impl) {
      this.impl_ = impl;
      this.Root = new BetterTreeNode<T>(null, impl.Nodes);

      this.impl_.AfterSelect += (sender, args) =>
          this.Selected.Invoke(
              (BetterTreeNode<T>) this.impl_.SelectedNode!.Tag);

      this.impl_.TreeViewNodeSorter = this.comparer_;
    }

    public void BeginUpdate() {
      this.impl_.BeginUpdate();
      this.comparer_.Enabled = false;
    }

    public void EndUpdate() {
      this.impl_.EndUpdate();
      this.comparer_.Enabled = true;
    }

    private class BetterTreeViewComparer : IComparer {
      private readonly IComparer<BetterTreeNode<T>> defaultComparer_ =
          new DefaultBetterTreeViewComparer();

      public bool Enabled { get; set; }

      public IComparer<BetterTreeNode<T>>? Comparer { get; set; }

      public int Compare(object lhs, object rhs) {
        if (!this.Enabled) {
          return 0;
        }

        var comparer = this.Comparer ?? this.defaultComparer_;
        return comparer.Compare((BetterTreeNode<T>) (((TreeNode) lhs).Tag),
                                (BetterTreeNode<T>) (((TreeNode) rhs).Tag));
      }

      private class DefaultBetterTreeViewComparer :
          IComparer<BetterTreeNode<T>> {
        public int Compare(BetterTreeNode<T> lhs, BetterTreeNode<T> rhs)
          => lhs.AbsoluteIndex.CompareTo(rhs.AbsoluteIndex);
      }
    }
  }
}