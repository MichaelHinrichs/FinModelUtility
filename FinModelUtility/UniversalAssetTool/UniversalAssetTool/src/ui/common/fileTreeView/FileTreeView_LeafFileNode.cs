using fin.io.bundles;

namespace uni.ui.common.fileTreeView {
  public abstract partial class FileTreeView<TFiles> {
    protected class LeafFileNode : BFileNode, IFileTreeLeafNode {
      public LeafFileNode(ParentFileNode parent, IFileBundle file) : base(
          parent,
          file.DisplayName) {
        this.File = file;
        this.InitializeFilterNode(parent);

        this.treeNode.ClosedImage =
            this.treeNode.OpenImage =
                this.treeView.GetImageForFile(this.File);
      }

      public IFileBundle File { get; }
      public override string FullName => this.File.TrueFullPath;
    }
  }
}