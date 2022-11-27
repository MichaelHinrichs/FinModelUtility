using uni.platforms;

using cmb.api;
using fin.io.bundles;
using uni.platforms.threeDs;


namespace uni.games.great_ace_attorney {
  public class GreatAceAttorneyModelFileGatherer 
      : IFileBundleGatherer<CmbModelFileBundle> {
    public IFileBundleDirectory<CmbModelFileBundle>? GatherFileBundles(
        bool assert) {
      var greatAceAttorneyRom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "great_ace_attorney.cia", assert);
      if (greatAceAttorneyRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              greatAceAttorneyRom);

      return null;
    }
  }
}