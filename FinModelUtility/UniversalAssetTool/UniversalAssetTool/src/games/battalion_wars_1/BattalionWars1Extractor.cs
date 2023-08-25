using modl.api;

namespace uni.games.battalion_wars_1 {
  public class BattalionWars1Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new BattalionWars1FileGatherer(),
                                  new BattalionWarsModelLoader());
  }
}