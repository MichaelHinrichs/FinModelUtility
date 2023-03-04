using fin.io;
using fin.io.bundles;

using geo.api;

using uni.platforms.desktop;


namespace uni.games.dead_space_1 {
  public class DeadSpace1FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!SteamUtils.TryGetGameDirectory("Dead Space",
                                          out var deadSpaceDir,
                                          assert)) {
        return Enumerable.Empty<IFileBundle>();
      }

      var originalGameFileHierarchy = new FileHierarchy(deadSpaceDir);

      var baseOutputDirectory =
          GameFileHierarchyUtil.GetWorkingDirectoryForDirectory(
              originalGameFileHierarchy.Root,
              "dead_space_1");
      if (!baseOutputDirectory.Exists) {
        var strExtractor = new StrExtractor();
        foreach (var strFile in originalGameFileHierarchy.SelectMany(
                     dir => dir.FilesWithExtensionRecursive(".str"))) {
          strExtractor.Extract(strFile, baseOutputDirectory);
        }
      }

      var assetFileHierarchy = new FileHierarchy(baseOutputDirectory);
      return assetFileHierarchy
             .SelectMany(dir => dir.FilesWithExtension(".geo"))
             .Select(
                 geoFile => new GeoModelFileBundle { GeoFile = geoFile });
    }
  }
}