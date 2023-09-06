using fin.data.fuzzy;


namespace uni.ui.common.fileTreeView {
  public abstract partial class FileTreeView<TFile, TFiles> {
    protected class FileNode : IFileTreeNode<TFile> {
      private readonly IFileTreeView<TFile> treeview_;
      private readonly IBetterTreeNode<FileNode> treeNode_;
      private readonly IFuzzyNode<FileNode> filterNode_;

      public FileNode(FileTreeView<TFile, TFiles> treeView) {
        this.treeview_ = treeView;
        this.treeNode_ = treeView.betterTreeView_.Root;
        this.treeNode_.Data = this;

        this.filterNode_ = treeView.filterImpl_.Root.AddChild(this);

        this.InitDirectory_();
      }

      private FileNode(FileNode parent, TFile file) {
        this.File = file;
        this.FullName = file.TrueFullPath;

        this.treeview_ = parent.treeview_;
        this.treeNode_ =
            parent.treeNode_.Add(file.DisplayName);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitFile_();
      }

      private FileNode(FileNode parent, string text) {
        this.treeview_ = parent.treeview_;
        this.treeNode_ = parent.treeNode_.Add(text);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitDirectory_();
      }

      private void InitDirectory_() {
        this.treeNode_.ClosedImage = Icons.folderClosedImage;
        this.treeNode_.OpenImage = Icons.folderOpenImage;
      }

      private void InitFile_()
        => this.treeNode_.ClosedImage =
            this.treeNode_.OpenImage =
                this.treeview_.GetImageForFile(this.File!);

      public string Text => this.treeNode_.Text ?? "n/a";

      public TFile? File { get; set; }
      public string FullName { get; set; }

      public IFileTreeNode<TFile>? Parent => this.treeNode_.Parent?.Data;
      public float Similarity => this.filterNode_.Similarity;
      public float ChangeDistance => this.filterNode_.ChangeDistance;
      public IReadOnlySet<string> Keywords => this.filterNode_.Keywords;

      public FileNode AddChild(TFile file) => new(this, file);
      public FileNode AddChild(string text) => new(this, text);

      public IEnumerable<IFileTreeNode<TFile>> Children
        => this.filterNode_.Children.Select(fuzzyNode => fuzzyNode.Data);

      public void Expand() => this.treeNode_.Expand();
    }
  }
}