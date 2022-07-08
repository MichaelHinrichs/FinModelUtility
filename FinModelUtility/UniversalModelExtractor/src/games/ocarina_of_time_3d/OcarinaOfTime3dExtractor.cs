using cmb.api;


namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new OcarinaOfTime3dFileGatherer(),
                                  new CmbModelLoader());
  }
}