using fin.io;
using fin.io.bundles;

namespace uni.ui.common.fileTreeView {
  public abstract partial class FileTreeView<TFiles> {
    protected class ParentFileNode : BFileNode, IFileTreeParentNode {
      public ParentFileNode(FileTreeView<TFiles> treeView) : base(
          treeView) {
        this.InitializeFilterNode(treeView.filterImpl_.Root);
        this.InitDirectory_();
      }

      private ParentFileNode(ParentFileNode parent, string text) : base(
          parent,
          text) {
        this.InitializeFilterNode(parent);
        this.InitDirectory_();
      }

      private void InitDirectory_() {
        this.treeNode.ClosedImage = Icons.folderClosedImage;
        this.treeNode.OpenImage = Icons.folderOpenImage;
      }

      public IFileHierarchyDirectory? Directory { get; set; }
      public override string? FullName => this.Directory?.FullPath;

      public ParentFileNode AddChild(string text) => new(this, text);
      public LeafFileNode AddChild(IFileBundle file) => new(this, file);

      public IEnumerable<IFileTreeNode> ChildNodes
        => this.filterNode.Children.Select(fuzzyNode => fuzzyNode.Data);

      public IEnumerable<IFileBundle> GetFiles(
          bool recursive) {
        var children = this.ChildNodes.OfType<IFileTreeLeafNode>()
                           .Select(fileNode => fileNode.File);
        return !recursive
            ? children
            : children.Concat(
                this.ChildNodes
                    .OfType<IFileTreeParentNode>()
                    .SelectMany(parentNode
                                    => parentNode
                                        .GetFiles(
                                            true)));
      }

      public IEnumerable<TSpecificFile> GetFilesOfType<TSpecificFile>(
          bool recursive) where TSpecificFile : IFileBundle
        => this.GetFiles(recursive).OfType<TSpecificFile>();

      public void Expand() => this.treeNode.Expand();
    }
  }
}