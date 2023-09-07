using cmb.api;

namespace uni.games.majoras_mask_3d {
  public class MajorasMask3dExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new MajorasMask3dAnnotatedFileGatherer(),
                                  new CmbModelImporter());
  }
}