using fin.io;
using fin.model;
using level5.api;
using modl.api;
using uni.platforms;
using uni.platforms.threeDs;
using uni.platforms.threeDs.tools;
using uni.util.io;


namespace uni.games.professor_layton_vs_phoenix_wright {
  public class
      ProfessorLaytonVsPhoenixWrightModelFileGatherer : IModelFileGatherer<
          XcModelFileBundle> {
    public IModelDirectory<XcModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var professorLaytonVsPhoenixWrightRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "professor_layton_vs_phoenix_wright.cia");
      if (professorLaytonVsPhoenixWrightRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              professorLaytonVsPhoenixWrightRom);

      if (new ThreeDsXfsaTool().Extract(
              fileHierarchy.Root.Files.Single(file => file.Name == "vs1.fa"))) {
        fileHierarchy.Root.Refresh(true);
      }

      return new FileHierarchyBundler<XcModelFileBundle>(
          directory => {
            var xcFiles = directory.FilesWithExtension(".xc")
                                   .ToDictionary(
                                       xcFile => xcFile.NameWithoutExtension);

            var animationXcFiles =
                xcFiles
                    .Where(xcFile => xcFile.Key.EndsWith("_mn"))
                    .ToArray();

            var animationAndModelXcFiles =
                animationXcFiles
                    .Select<KeyValuePair<string, IFileHierarchyFile>, (
                        IFileHierarchyFile,
                        IFileHierarchyFile)?>(animationXcFile => {
                      if (xcFiles.TryGetValue(animationXcFile.Key[..^3],
                                              out var modelXcFile)) {
                        return (modelXcFile, animationXcFile.Value);
                      }
                      return null;
                    })
                    .Where(value => value != null)
                    .ToArray();

            var bundles =
                animationAndModelXcFiles
                    .Select(
                        animationAndModelXcFile => {
                          var (modelXcFile, animationXcFile) = animationAndModelXcFile.Value;
                          return new XcModelFileBundle {
                              ModelXcFile = modelXcFile,
                              AnimationXcFile = animationXcFile,
                          };
                        })
                    .ToList();

            return bundles;
          }
      ).GatherBundles(fileHierarchy);
    }
  }
}