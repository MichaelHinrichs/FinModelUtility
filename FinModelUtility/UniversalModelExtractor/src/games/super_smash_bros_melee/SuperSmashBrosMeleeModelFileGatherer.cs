using dat.api;

using fin.model;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.super_smash_bros_melee {
  public class
      SuperSmashBrosMeleeModelFileGatherer : IModelFileGatherer<
          DatModelFileBundle> {
    public IModelDirectory<DatModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var superSmashBrosMeleeRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "super_smash_bros_melee.gcm");
      if (superSmashBrosMeleeRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, superSmashBrosMeleeRom);

      var rootModelDirectory =
          new ModelDirectory<DatModelFileBundle>("super_smash_bros_melee");

      foreach (var datFile in fileHierarchy.Root.FilesWithExtension(".dat")) {
        rootModelDirectory.AddFileBundle(new DatModelFileBundle(datFile));
      }

      return rootModelDirectory;
    }
  }
}