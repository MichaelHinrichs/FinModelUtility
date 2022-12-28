using j3d.exporter;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new SuperMarioSunshineModelFileGatherer(),
                                  new BmdModelLoader());
  }
}