using modl.api;


namespace uni.games.battalion_wars {
  public class BattalionWarsExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new BattalionWarsFileGatherer(),
                                  new ModlModelLoader());
  }
}