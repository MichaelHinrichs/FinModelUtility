using modl.api;


namespace uni.games.battalion_wars_1 {
  public class BattalionWars1Extractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new BattalionWars1FileGatherer(),
                                  new ModlModelLoader());
  }
}