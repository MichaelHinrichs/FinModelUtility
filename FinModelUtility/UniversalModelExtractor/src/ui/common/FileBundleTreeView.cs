using fin.io.bundles;


namespace uni.ui.common {
  public class FileBundleTreeView
      : FileTreeView<IFileBundle, RootFileBundleDirectory> {
    protected override void PopulateImpl(RootFileBundleDirectory directoryRoot,
                                         FileNode uiRoot) {
      foreach (var subdir in directoryRoot.Subdirs) {
        this.AddDirectoryToNode_(subdir, uiRoot);
      }
    }

    private FileNode AddDirectoryToNode_(IFileBundleDirectory directory,
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