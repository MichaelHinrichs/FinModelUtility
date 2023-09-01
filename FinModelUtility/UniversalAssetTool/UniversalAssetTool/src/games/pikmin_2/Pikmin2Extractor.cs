using j3d.api;

namespace uni.games.pikmin_2 {
  public class Pikmin2Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new Pikmin2FileGatherer(),
                                  new BmdModelReader());
  }
}