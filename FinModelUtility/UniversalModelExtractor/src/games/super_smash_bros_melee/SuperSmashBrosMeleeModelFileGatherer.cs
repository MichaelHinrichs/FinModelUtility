using bmd.exporter;

using fin.model;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.super_smash_bros_melee {
  public class
      SuperSmashBrosMeleeModelFileGatherer : IModelFileGatherer<
          BmdModelFileBundle> {
    public IModelDirectory<BmdModelFileBundle>? GatherModelFileBundles(
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

      return null;
    }
  }
}