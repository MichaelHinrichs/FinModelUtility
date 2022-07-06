using fin.data;
using fin.io;
using fin.model;


namespace uni.util.io {
  public class FileHierarchyBundler<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    private readonly
        Func<IFileHierarchyDirectory, IEnumerable<TModelFileBundle>?> handler_;

    public FileHierarchyBundler(
        Func<IFileHierarchyDirectory, IEnumerable<TModelFileBundle>?> handler) {
      this.handler_ = handler;
    }

    public IModelDirectory<TModelFileBundle> GatherBundles(
        IFileHierarchy fileHierarchy) {
      var fileHierarchyRoot = fileHierarchy.Root;
      var rootModelDirectory =
          new ModelDirectory<TModelFileBundle>(fileHierarchy.Root.Name);

      var lazyFileHierarchyDirToBundleDir =
          new LazyDictionary<IFileHierarchyDirectory,
              IModelDirectory<TModelFileBundle>>(
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