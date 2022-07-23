using fin.log;
using fin.model;

using modl.api;

using uni.platforms;
using uni.platforms.gcn;
using uni.util.io;


namespace uni.games.battalion_wars {
  public class
      BattalionWarsFileGatherer : IModelFileGatherer<ModlModelFileBundle> {
    public IModelDirectory<ModlModelFileBundle>? GatherModelFileBundles(
        bool assert) {
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

      foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles = directory.FilesWithExtension(".res");
        foreach (var resFile in resFiles) {
          didUpdate |= new ResDump().Run(resFile);
        }

        if (didUpdate) {
          directory.Refresh();
        }
      }

      return new FileHierarchyBundler<ModlModelFileBundle>(
          directory => {
            var modlFiles = directory.FilesWithExtension(".modl");
            var animFiles = directory.FilesWithExtension(".anim");

            return modlFiles.Select(
                modlFile =>
                    new ModlModelFileBundle {
                        ModlFile = modlFile,
                        AnimFiles =
                            modlFile.NameWithoutExtension == "WGRUNT"
                                ? animFiles
                                  .Where(animFile => animFile.NameWithoutExtension == "FGRUN")
                                             //animFile.Name.StartsWith("FG"))
                                  .ToArray()
                                : null
                    });
          }
      ).GatherBundles(fileHierarchy);
    }
  }
}