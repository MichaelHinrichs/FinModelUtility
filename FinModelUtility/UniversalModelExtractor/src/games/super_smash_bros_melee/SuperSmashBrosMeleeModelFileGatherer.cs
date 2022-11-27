using dat.api;
using fin.io.bundles;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.super_smash_bros_melee {
  public class SuperSmashBrosMeleeModelFileGatherer 
      : IFileBundleGatherer<DatModelFileBundle> {
    public IFileBundleDirectory<DatModelFileBundle>? GatherFileBundles(
        bool assert) {
      var superSmashBrosMeleeRom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "super_smash_bros_melee.gcm", assert);
      if (superSmashBrosMeleeRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, superSmashBrosMeleeRom);

      var rootModelDirectory =
          new FileBundleDirectory<DatModelFileBundle>("super_smash_bros_melee");

      foreach (var datFile in fileHierarchy.Root.FilesWithExtension(".dat")) {
        var datFileName = datFile.NameWithoutExtension;

        var isValidModel = false;

        // Playable character data
        if (datFileName.StartsWith("Pl")) {
          if (datFileName.EndsWith("Nr")) {
            isValidModel = true;
          }
        }

        // Stages
        if (datFileName.StartsWith("Gr")) {
          isValidModel = true;
        }

        // Trophies
        if (datFileName.StartsWith("Ty")) {
          isValidModel = true;
        }

        // TODO: Look into the other files

        if (isValidModel) {
          rootModelDirectory.AddFileBundle(new DatModelFileBundle(datFile));
        }
      }

      return rootModelDirectory;
    }
  }
}