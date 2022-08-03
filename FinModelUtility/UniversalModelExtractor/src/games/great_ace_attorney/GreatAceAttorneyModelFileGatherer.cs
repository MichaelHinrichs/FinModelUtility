using fin.io;
using fin.model;

using uni.platforms;

using cmb.api;

using uni.platforms.threeDs;


namespace uni.games.great_ace_attorney {
  public class
      GreatAceAttorneyModelFileGatherer : IModelFileGatherer<
          CmbModelFileBundle> {
    public IModelDirectory<CmbModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var greatAceAttorneyRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "great_ace_attorney.cia");
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