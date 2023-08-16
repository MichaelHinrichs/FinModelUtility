using mod.cli;

namespace uni.games.pikmin_1 {
  public class Pikmin1Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new Pikmin1ModelFileGatherer(),
                                  new ModModelLoader());
  }
}