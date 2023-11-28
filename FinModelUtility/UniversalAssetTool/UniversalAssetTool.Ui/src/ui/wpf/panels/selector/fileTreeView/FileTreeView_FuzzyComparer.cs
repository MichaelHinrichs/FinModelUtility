using fin.util.asserts;

using uni.ui.common;

namespace uni.ui.wpf.panels.selector.fileTreeView {
  public abstract partial class FileTreeView {
    private class FuzzyTreeComparer : IComparer<IBetterTreeNode<BFileNode>> {
      public int Compare(IBetterTreeNode<BFileNode> lhs,
                         IBetterTreeNode<BFileNode> rhs)
        => -Asserts.CastNonnull(lhs.Data)
                   .Similarity.CompareTo(
                       Asserts.CastNonnull(rhs.Data).Similarity);
    }
  }
}