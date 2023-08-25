using j3d.exporter;

namespace uni.games.pikmin_2 {
  public class Pikmin2Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new Pikmin2FileGatherer(),
                                  new BmdModelLoader());
  }
}