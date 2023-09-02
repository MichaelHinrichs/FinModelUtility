using mod.cli;

namespace uni.games.pikmin_1 {
  public class Pikmin1Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new Pikmin1ModelFileGatherer(),
                                  new ModModelImporter());
  }
}