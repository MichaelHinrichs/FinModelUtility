using fin.model;


namespace uni.ui.common {
  public class
      ModelFileTreeView : FileTreeView<IModelFileBundle, RootModelDirectory> {
    protected override void PopulateImpl(RootModelDirectory directoryRoot,
                                         FileNode uiRoot) {
      foreach (var subdir in directoryRoot.Subdirs) {
        this.AddDirectoryToNode_(subdir, uiRoot);
      }
    }

    private FileNode AddDirectoryToNode_(IModelDirectory directory,
                                         FileNode parentNode) {
      var uiNode = parentNode.AddChild(directory.Name);

      foreach (var subdirectory in directory.Subdirs) {
        this.AddDirectoryToNode_(subdirectory, uiNode);
      }

      foreach (var fileBundle in directory.FileBundles) {
        uiNode.AddChild(fileBundle);
      }

      if (DebugFlags.OPEN_DIRECTORIES_BY_DEFAULT) {
        uiNode.Expand();
      }

      return uiNode;
    }
  }
}