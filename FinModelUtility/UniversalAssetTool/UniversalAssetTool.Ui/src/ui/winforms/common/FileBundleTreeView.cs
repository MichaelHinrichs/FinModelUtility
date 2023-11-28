using System.Drawing;

using fin.audio.io;
using fin.io.bundles;
using fin.model.io;
using fin.scene;

using uni.ui.common;
using uni.ui.winforms.common.fileTreeView;

namespace uni.ui.winforms.common {
  public class FileBundleTreeView : FileTreeView<IFileBundleDirectory> {
    protected override void PopulateImpl(IFileBundleDirectory directoryRoot,
                                         ParentFileNode uiRoot) {
      foreach (var subdir in directoryRoot.Subdirs) {
        this.AddDirectoryToNode_(subdir, uiRoot);
      }
    }

    private void AddDirectoryToNode_(IFileBundleDirectory directory,
                                     ParentFileNode parentNode) {
      var uiNode = parentNode.AddChild(directory.Name);
      uiNode.Directory = directory.Directory;

      foreach (var subdirectory in directory.Subdirs) {
        this.AddDirectoryToNode_(subdirectory, uiNode);
      }

      foreach (var fileBundle in directory.FileBundles) {
        uiNode.AddChild(fileBundle);
      }

      if (DebugFlags.OPEN_DIRECTORIES_BY_DEFAULT) {
        uiNode.Expand();
      }
    }

    public override Image GetImageForFile(IFileBundle file)
      => file switch {
          IModelFileBundle => Icons.modelImage,
          IAudioFileBundle => Icons.musicImage,
          ISceneFileBundle => Icons.sceneImage,
      };
  }
}