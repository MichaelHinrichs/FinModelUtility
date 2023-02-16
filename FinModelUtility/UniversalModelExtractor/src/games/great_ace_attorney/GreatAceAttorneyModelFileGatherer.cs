using uni.platforms;

using cmb.api;

using fin.io.bundles;

using uni.platforms.threeDs;


namespace uni.games.great_ace_attorney {
  public class GreatAceAttorneyModelFileGatherer
      : IFileBundleGatherer<CmbModelFileBundle> {
    public IEnumerable<CmbModelFileBundle> GatherFileBundles(bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "great_ace_attorney.cia",
              assert,
              out var greatAceAttorneyRom)) {
        yield break;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              greatAceAttorneyRom);

      yield break;
    }
  }
}