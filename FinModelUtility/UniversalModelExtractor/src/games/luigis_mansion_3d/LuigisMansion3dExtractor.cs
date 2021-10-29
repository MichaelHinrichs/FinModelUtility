using fin.log;

using uni.platforms;
using uni.platforms.threeDs;

namespace uni.games.luigis_mansion_3d {
  public class LuigisMansion3dExtractor {
    private readonly ILogger logger_ =
        Logging.Create<LuigisMansion3dExtractor>();

    public void ExtractAll() {
      var luigisMansionRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "luigis_mansion_3d.cia");

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              luigisMansionRom);
    }
  }
}