using f3dzex2.io;

using Quad64.memory;
using Quad64.src.LevelInfo;


namespace Quad64.src.Scripts {
  public interface IGeoScripts {
    void parse(
        IReadOnlySm64Memory n64Memory,
        Model3DLods mdlLods,
        ref Level lvl,
        byte seg,
        uint off);
  }

  public class GeoScriptsWrapper : IGeoScripts {
    private readonly IGeoScripts geoScriptsImplementation_ = new GeoScriptsV2();

    public void parse(
        IReadOnlySm64Memory n64Memory,
        Model3DLods mdlLods,
        ref Level lvl,
        byte seg,
        uint off)
      => this.geoScriptsImplementation_.parse(n64Memory,
                                              mdlLods,
                                              ref lvl,
                                              seg,
                                              off);
  }
}