using cmb.api;

namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new OcarinaOfTime3dFileGatherer(),
                                  new CmbModelImporter());
  }
}