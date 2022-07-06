using fin.log;
using fin.model;

using modl.api;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.battalion_wars {
  public class
      BattalionWarsFileGatherer : IModelFileGatherer<ModlModelFileBundle> {
    private readonly ILogger logger_ =
        Logging.Create<BattalionWarsFileGatherer>();

    public IModelDirectory<ModlModelFileBundle>? GatherModelFileBundles(bool assert) {
      var battalionWarsRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "battalion_wars.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              battalionWarsRom);

      foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles = directory.FilesWithExtension(".res");
        foreach (var resFile in resFiles) {
          didUpdate |= new ResDump().Run(resFile);
        }
      }

      // TODO: Use Modl2Fbx

      return null;
    }
  }
}