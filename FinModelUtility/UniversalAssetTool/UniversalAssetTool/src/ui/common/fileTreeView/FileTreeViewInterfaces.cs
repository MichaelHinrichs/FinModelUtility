namespace uni.ui.common.fileTreeView {
  public interface IFileTreeView<TFile> {
    public delegate void FileSelectedHandler(IFileTreeNode<TFile> fileNode);

    event FileSelectedHandler FileSelected;


    public delegate void DirectorySelectedHandler(
        IFileTreeNode<TFile> directoryNode);

    event DirectorySelectedHandler DirectorySelected;

    Image GetImageForFile(TFile file);
  }

  public interface IFileTreeNode<TFile> {
    string Text { get; }

    TFile? File { get; }

    IFileTreeNode<TFile>? Parent { get; }
    IEnumerable<IFileTreeNode<TFile>> Children { get; }
  }
}
