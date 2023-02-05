using cmb.api;


namespace uni.games.luigis_mansion_3d {
  public class LuigisMansion3dExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new LuigisMansion3dModelFileGatherer(),
                                  new CmbModelLoader());
  }
}