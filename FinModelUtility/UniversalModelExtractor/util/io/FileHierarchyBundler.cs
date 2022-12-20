using fin.data;
using fin.io;
using fin.io.bundles;


namespace uni.util.io {
  public class FileHierarchyBundler<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly
        Func<IFileHierarchyDirectory, IEnumerable<TFileBundle>?> handler_;

    public FileHierarchyBundler(
        Func<IFileHierarchyDirectory, IEnumerable<TFileBundle>?> handler) {
      this.handler_ = handler;
    }

    public IFileBundleDirectory<TFileBundle> GatherBundles(
        IFileHierarchy fileHierarchy) {
      var fileHierarchyRoot = fileHierarchy.Root;
      var rootModelDirectory =
          new FileBundleDirectory<TFileBundle>(fileHierarchy.Root.Name);

      var lazyFileHierarchyDirToBundleDir =
          new LazyDictionary<IFileHierarchyDirectory,
              IFileBundleDirectory<TFileBundle>>(
              (lazyDict, dir) => {
                var parent = dir.Parent != null
                                 ? lazyDict[dir.Parent]
                                 : rootModelDirectory;
                return parent.AddSubdir(dir.Name);
              });

      lazyFileHierarchyDirToBundleDir[fileHierarchyRoot] = rootModelDirectory;

      foreach (var directory in fileHierarchy) {
        var bundles = handler_(directory);
        if (bundles == null) {
          continue;
        }

        foreach (var bundle in bundles) {
          lazyFileHierarchyDirToBundleDir[directory].AddFileBundle(bundle);
        }
      }

      return rootModelDirectory;
    }
  }
}