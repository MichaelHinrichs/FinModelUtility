namespace uni.ui.common.fileTreeView {
  public interface IFileTreeView<TFile> {
    public delegate void FileSelectedHandler(IFileTreeLeafNode<TFile> fileNode);

    event FileSelectedHandler FileSelected;


    public delegate void DirectorySelectedHandler(
        IFileTreeParentNode<TFile> directoryNode);

    event DirectorySelectedHandler DirectorySelected;

    Image GetImageForFile(TFile file);
  }

  public interface IFileTreeNode<TFile> {
    string Text { get; }
    IFileTreeParentNode<TFile>? Parent { get; }
  }

  public interface IFileTreeParentNode<TFile> : IFileTreeNode<TFile> {
    IEnumerable<IFileTreeNode<TFile>> ChildNodes { get; }

    IEnumerable<TFile> GetFiles(bool recursive);
    IEnumerable<TSpecificFile> GetFilesOfType<TSpecificFile>(bool recursive);
  }

  public interface IFileTreeLeafNode<TFile> : IFileTreeNode<TFile> {
    TFile File { get; }
  }
}