using fin.data;
using fin.io;
using fin.io.bundles;


namespace uni.util.io {
  public class FileBundleHierarchyOrganizer {
    public IFileBundleDirectory Organize(
        IEnumerable<IFileBundle> fileBundles) {
      var rootFileBundleDirectory = new FileBundleDirectory("(root)");

      var lazyFileHierarchyDirToBundleDir =
          new LazyDictionary<IFileHierarchyDirectory, IFileBundleDirectory>(
              (lazyDict, dir) => {
                if (dir.Parent == null) {
                  return rootFileBundleDirectory.AddSubdir(dir.Root.Name);
                }

                return lazyDict[dir.Parent].AddSubdir(dir.Name);
              });

      foreach (var fileBundle in fileBundles) {
        lazyFileHierarchyDirToBundleDir[fileBundle.Directory]
            .AddFileBundle(fileBundle);
      }

      rootFileBundleDirectory.CleanUp();
      return rootFileBundleDirectory;
    }
  }
}