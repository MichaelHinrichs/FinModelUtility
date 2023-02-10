using fin.audio;
using fin.io.bundles;
using fin.model;
using fin.scene;


namespace uni.ui.common {
  public class FileBundleTreeView
      : FileTreeView<IFileBundle, IFileBundleDirectory> {
    protected override void PopulateImpl(IFileBundleDirectory directoryRoot,
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

    public override Image GetImageForFile(IFileBundle file)
      => file switch {
          IModelFileBundle => Icons.modelImage,
          IAudioFileBundle => Icons.musicImage,
          ISceneFileBundle => Icons.sceneImage,
      };
  }
}