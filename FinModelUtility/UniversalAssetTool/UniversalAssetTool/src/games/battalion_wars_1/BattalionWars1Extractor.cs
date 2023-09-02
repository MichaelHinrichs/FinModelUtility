using modl.api;

namespace uni.games.battalion_wars_1 {
  public class BattalionWars1Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new BattalionWars1FileGatherer(),
                                  new BattalionWarsModelImporter());
  }
}