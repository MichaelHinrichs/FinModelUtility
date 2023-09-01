using j3d.api;

namespace uni.games.mario_kart_double_dash {
  public class MarioKartDoubleDashExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new MarioKartDoubleDashFileGatherer(),
                                  new BmdModelReader());
  }
}