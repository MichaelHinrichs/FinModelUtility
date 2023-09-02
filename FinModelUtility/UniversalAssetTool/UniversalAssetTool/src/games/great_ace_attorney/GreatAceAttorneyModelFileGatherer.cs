using cmb.api;

using fin.io.bundles;

using uni.platforms.threeDs;

namespace uni.games.great_ace_attorney {
  public class GreatAceAttorneyModelFileGatherer
      : IFileBundleGatherer<CmbModelFileBundle> {
    public IEnumerable<CmbModelFileBundle> GatherFileBundles() {
      if (!new ThreeDsFileHierarchyExtractor().TryToExtractFromGame(
              "great_ace_attorney",
              out var fileHierarchy)) {
        yield break;
      }

      yield break;
    }
  }
}