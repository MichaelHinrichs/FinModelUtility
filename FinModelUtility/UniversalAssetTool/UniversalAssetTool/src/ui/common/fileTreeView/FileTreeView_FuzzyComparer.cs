using fin.util.asserts;

namespace uni.ui.common.fileTreeView {
  public abstract partial class FileTreeView<TFile, TFiles> {
    private class FuzzyTreeComparer : IComparer<IBetterTreeNode<FileNode>> {
      public int Compare(
          IBetterTreeNode<FileNode> lhs,
          IBetterTreeNode<FileNode> rhs)
        => -Asserts.CastNonnull(lhs.Data)
                   .Similarity.CompareTo(
                       Asserts.CastNonnull(rhs.Data).Similarity);
    }
  }
}