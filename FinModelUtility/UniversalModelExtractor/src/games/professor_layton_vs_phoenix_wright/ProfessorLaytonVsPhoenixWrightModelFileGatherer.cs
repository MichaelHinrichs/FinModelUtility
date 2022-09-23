using fin.model;
using uni.platforms;
using uni.platforms.threeDs;
using uni.platforms.threeDs.tools;


namespace uni.games.professor_layton_vs_phoenix_wright {
  public class
      ProfessorLaytonVsPhoenixWrightModelFileGatherer : IModelFileGatherer<
          IModelFileBundle> {
    public IModelDirectory<IModelFileBundle>? GatherModelFileBundles(
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

      var rootModelDirectory =
          new ModelDirectory<IModelFileBundle>(
              "professor_layton_vs_phoenix_wright");

      new ThreeDsXfsaTool().Extract(
          fileHierarchy.Root.Files.Single(file => file.Name == "vs1.fa"));

      return rootModelDirectory;
    }
  }
}