using System.Collections;


namespace uni.ui.common {
  public static class BetterTreeUtil {
    public static void ForEach<T, S>(TreeNodeCollection collection,
                                     Action<T> callback)
        where T : IBetterTreeNode<S>
        where S : class {
      // TODO: Remove unboxing?
      foreach (var childTreeNodeObj in collection) {
        var childTreeNode = (TreeNode)childTreeNodeObj;
        var childBetterTreeNode =
            BetterTreeUtil.GetBetterFrom<T, S>(childTreeNode);

        callback(childBetterTreeNode);
      }
    }

    public static T GetBetterFrom<T, S>(TreeNode node)
        where T : IBetterTreeNode<S>
        where S : class
      => (T)node.Tag;
  }

  public interface IBetterTreeView<T> where T : class {
    IBetterTreeNode<T> Root { get; }

    delegate void SelectedHandler(IBetterTreeNode<T> betterTreeNode);

    event SelectedHandler Selected;
  }


  public interface IBetterTreeNode<T> where T : class {
    TreeNode? Impl { get; }

    IBetterTreeView<T> Tree { get; }
    IBetterTreeNode<T>? Parent { get; }

    T? Data { get; set; }
    string? Text { get; }

    Image? OpenImage { set; }
    Image? ClosedImage { set; }

    int OpenImageIndex { get; set; }
    int ClosedImageIndex { get; set; }

    IBetterTreeNode<T> Add(string text);
    void Add(IBetterTreeNode<T> node);

    void ResetChildrenRecursively(
        Func<IBetterTreeNode<T>, bool>? filter = null);

    bool IsExpanded { get; }
    void Expand();
    void ExpandRecursively();
  }

  public class BetterTreeView<T> : IBetterTreeView<T> where T : class {
    // TODO: Add tests.
    // TODO: Add support for different hierarchies.

    private readonly TreeView impl_;
    private readonly Dictionary<Image, int> imageToIndex_ = new();

    private BetterTreeViewComparer comparer_ = new();

    public IBetterTreeNode<T> Root { get; }

    public event IBetterTreeView<T>.SelectedHandler Selected = delegate { };

    public IComparer<IBetterTreeNode<T>>? Comparer {
      get => this.comparer_.Comparer;
      set => this.comparer_.Comparer = value;
    }

    public BetterTreeView(TreeView impl) {
      this.impl_ = impl;
      this.Root = new BetterTreeNode(this, null, impl.Nodes);

      this.impl_.AfterSelect += (sender, args) =>
          this.Selected.Invoke(
              (BetterTreeNode)this.impl_.SelectedNode!.Tag);

      this.impl_.TreeViewNodeSorter = this.comparer_;

      this.impl_.AfterExpand += (sender, args) => {
        var node = args.Node!;
        var betterNode =
            BetterTreeUtil.GetBetterFrom<BetterTreeNode, T>(node);
        betterNode.IsExpanded = true;
        node.ImageIndex = node.SelectedImageIndex = betterNode.OpenImageIndex;
      };
      this.impl_.AfterCollapse += (sender, args) => {
        var node = args.Node!;
        var betterNode =
            BetterTreeUtil.GetBetterFrom<BetterTreeNode, T>(node);
        betterNode.IsExpanded = false;
        node.ImageIndex = node.SelectedImageIndex = betterNode.ClosedImageIndex;
      };

      this.impl_.ImageList ??= new ImageList();
    }

    public void BeginUpdate() {
      this.impl_.BeginUpdate();
      this.impl_.SuspendLayout();
      this.comparer_.Enabled = false;
      this.impl_.Sorted = false;
      //this.impl_.Scrollable = false;
      this.impl_.Enabled = false;
      this.impl_.Visible = false;
      this.impl_.Hide();
    }

    public void EndUpdate() {
      this.impl_.EndUpdate();
      this.impl_.ResumeLayout();
      this.comparer_.Enabled = true;
      this.impl_.Sorted = true;
      //this.impl_.Scrollable = true;
      this.impl_.Enabled = true;
      this.impl_.Visible = true;
      this.impl_.Show();
    }

    public void ScrollToTop() {
      var nodes = this.impl_.Nodes;
      if (nodes.Count > 0) {
        nodes[0].EnsureVisible();
      }
    }

    // TODO: Slow
    public int GetOrAddIndexOfImage(Image? image) {
      if (image == null) {
        return -1;
      }

      if (this.imageToIndex_.TryGetValue(image, out var index)) {
        return index;
      }

      var imageList = this.impl_.ImageList.Images;

      index = imageList.Count;
      imageList.Add(image);

      this.imageToIndex_[image] = index;

      return index;
    }


    private class BetterTreeNode : IBetterTreeNode<T> {
      private readonly TreeNodeCollection collection_;

      private int openImageIndex_ = -1;
      private int closedImageIndex_ = -1;

      public TreeNode? Impl { get; }

      public IBetterTreeView<T> Tree { get; }
      public IBetterTreeNode<T>? Parent { get; private set; }

      public int AbsoluteIndex { get; }

      private readonly List<BetterTreeNode> absoluteChildren_ = new();

      // TODO: Possible to remove unboxing?
      public T? Data { get; set; }
      public string? Text => this.Impl?.Text;

      public BetterTreeNode(IBetterTreeView<T> tree,
                            TreeNode? impl,
                            TreeNodeCollection collection) {
        this.Tree = tree;
        this.Impl = impl;

        if (impl != null) {
          impl.Tag = this;
        }

        this.collection_ = collection;

        this.AbsoluteIndex = impl?.Index ?? 0;
      }

      public Image? OpenImage {
        set => this.OpenImageIndex =
                   ((BetterTreeView<T>)this.Tree).GetOrAddIndexOfImage(value);
      }

      public Image? ClosedImage {
        set => this.ClosedImageIndex =
                   ((BetterTreeView<T>)this.Tree).GetOrAddIndexOfImage(value);
      }

      public int OpenImageIndex {
        get => this.openImageIndex_;
        set {
          this.openImageIndex_ = value;
          if (this.IsExpanded) {
            this.Impl.ImageIndex = this.Impl.SelectedImageIndex = value;
          }
        }
      }

      public int ClosedImageIndex {
        get => this.closedImageIndex_;
        set {
          this.closedImageIndex_ = value;
          if (this.IsExpanded) {
            this.Impl.ImageIndex = this.Impl.SelectedImageIndex = value;
          }
        }
      }

      public IBetterTreeNode<T> Add(string text) {
        var childTreeNode = this.collection_.Add(text);

        var childBetterTreeNode =
            new BetterTreeNode(this.Tree, childTreeNode, childTreeNode.Nodes) {
                Parent = this
            };

        this.absoluteChildren_.Add(childBetterTreeNode);

        return childBetterTreeNode;
      }

      public void Add(IBetterTreeNode<T> node) =>
          this.collection_.Add(node.Impl);

      public void ResetChildrenRecursively(
          Func<IBetterTreeNode<T>, bool>? filter = null) {
        this.ClearRecursively_();
        this.ReaddChildrenRecursively(filter);
        this.ExpandRecursively();
      }

      public bool IsExpanded { get; set; } = false;

      public void Expand() => this.Impl?.Expand();

      public void ExpandRecursively() {
        this.Expand();
        BetterTreeUtil.ForEach<IBetterTreeNode<T>, T>(
            this.collection_,
            childBetterTreeNode =>
                childBetterTreeNode.ExpandRecursively());
      }

      private void ClearRecursively_() {
        BetterTreeUtil.ForEach<BetterTreeNode, T>(
            this.collection_,
            childBetterTreeNode =>
                childBetterTreeNode.ClearRecursively_());

        this.collection_.Clear();
      }

      private void ReaddChildrenRecursively(
          Func<BetterTreeNode, bool>? filter) {
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


    private class BetterTreeViewComparer : IComparer {
      private readonly IComparer<IBetterTreeNode<T>> defaultComparer_ =
          new DefaultBetterTreeViewComparer();

      public bool Enabled { get; set; }

      public IComparer<IBetterTreeNode<T>>? Comparer { get; set; }

      public int Compare(object lhs, object rhs) {
        if (!this.Enabled) {
          return 0;
        }

        var comparer = this.Comparer ?? this.defaultComparer_;
        return comparer.Compare((BetterTreeNode)(((TreeNode)lhs).Tag),
                                (BetterTreeNode)(((TreeNode)rhs).Tag));
      }

      private class DefaultBetterTreeViewComparer :
          IComparer<IBetterTreeNode<T>> {
        public int Compare(IBetterTreeNode<T> lhs, IBetterTreeNode<T> rhs)
          => ((BetterTreeNode)lhs).AbsoluteIndex.CompareTo(
              ((BetterTreeNode)rhs).AbsoluteIndex);
      }
    }
  }
}