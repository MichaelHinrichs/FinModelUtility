using cmb.api;


namespace uni.games.luigis_mansion_3d {
  public class LuigisMansion3dExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new LuigisMansion3dModelFileGatherer(),
                                  new CmbModelLoader());
  }
}