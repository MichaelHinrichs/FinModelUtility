namespace uni.ui.common.fileTreeView {
  public abstract partial class FileTreeView<TFile, TFiles> {
    protected class LeafFileNode : BFileNode, IFileTreeLeafNode<TFile> {
      public LeafFileNode(ParentFileNode parent, TFile file) : base(
          parent,
          file.DisplayName) {
        this.File = file;
        this.InitializeFilterNode(parent);

        this.treeNode.ClosedImage =
            this.treeNode.OpenImage =
                this.treeView.GetImageForFile(this.File);
      }

      public TFile File { get; }
      public override string FullName => this.File.TrueFullPath;
    }
  }
}