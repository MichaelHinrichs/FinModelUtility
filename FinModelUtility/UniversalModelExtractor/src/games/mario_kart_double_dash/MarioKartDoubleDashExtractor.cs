using bmd.exporter;


namespace uni.games.mario_kart_double_dash {
  public class MarioKartDoubleDashExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new MarioKartDoubleDashFileGatherer(),
                                  new BmdModelLoader());
  }
}