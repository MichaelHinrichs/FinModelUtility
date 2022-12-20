using Quad64.src.LevelInfo;


namespace Quad64.src.Scripts {
  public interface IGeoScripts {
    void parse(Model3DLods mdlLods,
               ref Level lvl,
               byte seg,
               uint off,
               byte? areaID);
  }

  public class GeoScriptsWrapper : IGeoScripts {
    private readonly IGeoScripts geoScriptsImplementation_ = new GeoScriptsV2();

    public void parse(Model3DLods mdlLods,
                      ref Level lvl,
                      byte seg,
                      uint off,
                      byte? areaID)
      => this.geoScriptsImplementation_.parse(mdlLods, ref lvl, seg, off,
                                              areaID);
  }
}