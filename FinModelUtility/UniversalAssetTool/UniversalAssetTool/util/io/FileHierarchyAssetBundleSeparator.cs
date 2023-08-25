using fin.io;
using fin.io.bundles;

namespace uni.util.io {
  public class FileHierarchyAssetBundleSeparator<TFileBundle>
      : IFileBundleGatherer<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly IFileHierarchy fileHierarchy_;

    private readonly Func<IFileHierarchyDirectory, IEnumerable<TFileBundle>>
        handler_;

    public FileHierarchyAssetBundleSeparator(
        IFileHierarchy fileHierarchy,
        Func<IFileHierarchyDirectory, IEnumerable<TFileBundle>> handler) {
      this.fileHierarchy_ = fileHierarchy;
      this.handler_ = handler;
    }

    public IEnumerable<TFileBundle> GatherFileBundles(bool _)
      => this.fileHierarchy_.SelectMany(directory => this.handler_(directory));
  }
}