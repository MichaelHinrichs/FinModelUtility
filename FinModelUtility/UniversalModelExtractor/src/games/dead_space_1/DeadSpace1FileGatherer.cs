using fin.io;
using fin.io.bundles;

using geo.api;
using geo.schema.str;

using uni.platforms.desktop;


namespace uni.games.dead_space_1 {
  public class DeadSpace1FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!SteamUtils.TryGetGameDirectory("Dead Space",
                                          out var deadSpaceDir,
                                          assert)) {
        return Enumerable.Empty<IFileBundle>();
      }

      var fileHierarchy = new FileHierarchy(deadSpaceDir);
      foreach (var strFile in fileHierarchy.SelectMany(
                   dir => dir.FilesWithExtensionRecursive(".str"))) {
        var baseOutputDirectory =
            GameFileHierarchyUtil.GetWorkingDirectoryForFile(strFile);
        var outputDirForFile = new FinDirectory(
            Path.Join(baseOutputDirectory.FullName,
                      strFile.NameWithoutExtension));

        if (!outputDirForFile.Exists) {
          new StrExtractor().Extract(strFile, outputDirForFile);
        }
      }

      return Enumerable.Empty<IFileBundle>();
    }
  }
}