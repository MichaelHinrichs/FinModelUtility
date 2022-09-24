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
          XiModelFileBundle> {
    public IModelDirectory<XiModelFileBundle>? GatherModelFileBundles(
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

      return new FileHierarchyBundler<XiModelFileBundle>(
          directory => {
            var xcFiles = directory.FilesWithExtension(".xc");

            var bundles =
              xcFiles
                    .Select(
                      xcFile => new XiModelFileBundle {
                        XcFile = xcFile,
                      })
                    .ToList();

            return bundles;
          }
      ).GatherBundles(fileHierarchy);
    }
  }
}