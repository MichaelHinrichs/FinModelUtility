using System.Collections;


namespace uni.ui.common {
  public static class BetterTreeUtil {
    public static void ForEach<S>(TreeNodeCollection collection,
                                  Action<BetterTreeNode<S>> callback)
        where S : class {
      // TODO: Remove unboxing?
      foreach (var childTreeNodeObj in collection) {
        var childTreeNode = (TreeNode) childTreeNodeObj;
        var childBetterTreeNode =
            BetterTreeUtil.GetBetterFrom<S>(childTreeNode);

        callback(childBetterTreeNode);
      }
    }

    public static BetterTreeNode<S> GetBetterFrom<S>(TreeNode node)
        where S : class
      => (BetterTreeNode<S>) node.Tag;
  }

  public class BetterTreeNode<T> where T : class {
    private readonly TreeNode? impl_;
    private readonly TreeNodeCollection collection_;
    private int openImageIndex_;
    private int closedImageIndex_;

    public BetterTreeNode<T>? Parent { get; private set; }

    public int AbsoluteIndex { get; }

    private readonly IList<BetterTreeNode<T>> absoluteChildren_ =
        new List<BetterTreeNode<T>>();

    // TODO: Possible to remove unboxing?
    public T? Data { get; set; }
    public string? Text => this.impl_?.Text;

    public BetterTreeNode(TreeNode? impl, TreeNodeCollection collection) {
      this.impl_ = impl;

      if (impl != null) {
        impl.Tag = this;
      }

      this.collection_ = collection;

      this.AbsoluteIndex = impl?.Index ?? 0;
    }

    public int OpenImageIndex {
      get => this.openImageIndex_;
      set {
        this.openImageIndex_ = value;
        if (this.impl_?.IsExpanded ?? false) {
          this.impl_.ImageIndex = this.impl_.SelectedImageIndex = value;
        }
      }
    }

    public int ClosedImageIndex {
      get => this.closedImageIndex_;
      set {
        this.closedImageIndex_ = value;
        if (!(this.impl_?.IsExpanded ?? true)) {
          this.impl_.ImageIndex = this.impl_.SelectedImageIndex = value;
        }
      }
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
      this.ExpandRecursively();
    }

    public void ExpandRecursively() {
      this.impl_?.Expand();
      BetterTreeUtil.ForEach<T>(
          this.collection_,
          childBetterTreeNode =>
              childBetterTreeNode.ExpandRecursively());
    }

    private void ClearRecursively_() {
      BetterTreeUtil.ForEach<T>(
          this.collection_,
          childBetterTreeNode =>
              childBetterTreeNode.ClearRecursively_());

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

    public event SelectedHandler Selected = delegate { };

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

      this.impl_.AfterExpand += (sender, args) => {
        var node = args.Node!;
        var betterNode = BetterTreeUtil.GetBetterFrom<T>(node);
        node.ImageIndex = node.SelectedImageIndex = betterNode.OpenImageIndex;
      };
      this.impl_.AfterCollapse += (sender, args) => {
        var node = args.Node!;
        var betterNode = BetterTreeUtil.GetBetterFrom<T>(node);
        node.ImageIndex = node.SelectedImageIndex = betterNode.ClosedImageIndex;
      };
    }

    public void BeginUpdate() {
      this.impl_.BeginUpdate();
      this.comparer_.Enabled = false;
    }

    public void EndUpdate() {
      this.impl_.EndUpdate();
      this.comparer_.Enabled = true;
    }

    public void ScrollToTop() => this.impl_.Nodes[0].EnsureVisible();

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