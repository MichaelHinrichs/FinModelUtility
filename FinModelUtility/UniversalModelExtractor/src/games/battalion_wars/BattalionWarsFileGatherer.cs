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
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "battalion_wars.gcm");

      if (battalionWarsRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              battalionWarsRom);

      var rootModelDirectory =
          new ModelDirectory<ModlModelFileBundle>("battalion_wars");

      foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles = directory.FilesWithExtension(".res");
        foreach (var resFile in resFiles) {
          didUpdate |= new ResDump().Run(resFile);
        }

        if (didUpdate) {
          directory.Refresh();
        }

        var modlFiles = directory.FilesWithExtension(".modl");
        foreach (var modlFile in modlFiles) {
          rootModelDirectory.AddFileBundle(new ModlModelFileBundle {
              ModlFile = modlFile
          });
        }
      }

      return rootModelDirectory;
    }
  }
}