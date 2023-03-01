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

      var strExtractor = new StrExtractor();

      foreach (var strFile in originalGameFileHierarchy.SelectMany(
                   dir => dir.FilesWithExtensionRecursive(".str"))) {
        var baseOutputDirectory =
            GameFileHierarchyUtil.GetWorkingDirectoryForFile(
                strFile,
                "dead_space_1");
        var outputDirForFile = new FinDirectory(
            Path.Join(baseOutputDirectory.FullName,
                      strFile.NameWithoutExtension));

        if (!outputDirForFile.Exists) {
          strExtractor.Extract(strFile, outputDirForFile);
        }
      }

      var assetFileHierarchy =
          new FileHierarchy(
              GameFileHierarchyUtil.GetWorkingDirectoryForDirectory(
                  originalGameFileHierarchy.Root,
                  "dead_space_1"));

      return assetFileHierarchy
             .SelectMany(dir => dir.FilesWithExtension(".geo"))
             .Select(
                 geoFile => new GeoModelFileBundle { GeoFile = geoFile });
    }
  }
}