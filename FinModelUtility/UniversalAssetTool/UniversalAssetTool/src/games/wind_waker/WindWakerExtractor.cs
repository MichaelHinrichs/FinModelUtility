using j3d.exporter;

namespace uni.games.wind_waker {
  public class WindWakerExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new WindWakerFileGatherer(),
                                  new BmdModelReader());
  }
}