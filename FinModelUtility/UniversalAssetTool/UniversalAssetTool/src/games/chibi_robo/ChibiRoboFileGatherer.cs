using fin.io.bundles;

using uni.platforms.gcn;

namespace uni.games.chibi_robo {
  public class ChibiRoboAnnotatedFileGatherer : IAnnotatedFileBundleGatherer {
    public IEnumerable<IAnnotatedFileBundle> GatherFileBundles() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "chibi_robo",
              out var fileHierarchy)) {
        yield break;
      }

      yield break;
    }
  }
}