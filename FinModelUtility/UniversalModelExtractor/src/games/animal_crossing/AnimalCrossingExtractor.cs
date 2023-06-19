using fin.log;

using uni.platforms.gcn;

namespace uni.games.animal_crossing {
  public class AnimalCrossingExtractor : IExtractor {
    public void ExtractAll() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "animal_crossing",
              out var fileHierarchy)) {
        return;
      }

      var logger = Logging.Create<AnimalCrossingExtractor>();
      // TODO: Extract models via display lists
    }
  }
}