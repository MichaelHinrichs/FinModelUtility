using j3d.api;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new SuperMarioSunshineModelFileGatherer(),
                                  new BmdModelImporter());
  }
}