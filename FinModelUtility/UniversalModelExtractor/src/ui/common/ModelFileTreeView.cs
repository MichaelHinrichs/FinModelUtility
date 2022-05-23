using fin.games;


namespace uni.ui.common {
  public class
      ModelFileTreeView : FileTreeView<IModelFileBundle, IModelDirectory> {
    protected override void PopulateImpl(IModelDirectory directoryRoot,
                                         FileNode uiRoot)
      => this.AddDirectoryToNode_(directoryRoot, uiRoot);

    private FileNode AddDirectoryToNode_(IModelDirectory directory,
                                         FileNode parentNode) {
      var uiNode = parentNode.AddChild(directory.Name);

      foreach (var subdirectory in directory.Subdirectories) {
        this.AddDirectoryToNode_(subdirectory, uiNode);
      }

      foreach (var fileBundle in directory.FileBundles) {
        uiNode.AddChild(fileBundle);
      }

      return uiNode;
    }
  }
}