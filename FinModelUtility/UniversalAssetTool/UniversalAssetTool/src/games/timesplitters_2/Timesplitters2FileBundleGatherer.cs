using fin.io.bundles;
using fin.model.io;

using uni.platforms.gcn;

namespace uni.games.timesplitters_2 {
  public class Timesplitters2FileBundleGatherer
      : IAnnotatedFileBundleGatherer<IModelFileBundle> {
    public string Name => "timesplitters_2";

    public IEnumerable<IAnnotatedFileBundle<IModelFileBundle>>?
        GatherFileBundles() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "timesplitters_2",
              out var fileHierarchy)) {
        yield break;
      }
    }
  }
}