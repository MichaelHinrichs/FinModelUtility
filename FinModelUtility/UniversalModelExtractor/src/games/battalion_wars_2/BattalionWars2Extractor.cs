using modl.api;

namespace uni.games.battalion_wars_2 {
  public class BattalionWars2Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new BattalionWars2FileGatherer(),
                                  new BattalionWarsModelLoader());
  }
}