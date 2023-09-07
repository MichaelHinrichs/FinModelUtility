using j3d.api;

namespace uni.games.wind_waker {
  public class WindWakerExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new WindWakerAnnotatedFileGatherer(),
                                  new BmdModelImporter());
  }
}