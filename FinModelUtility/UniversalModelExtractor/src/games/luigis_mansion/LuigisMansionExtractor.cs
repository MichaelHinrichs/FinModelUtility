using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.luigis_mansion {
  public class LuigisMansionExtractor {
    private readonly ILogger logger_ =
        Logging.Create<LuigisMansionExtractor>();

    public void ExtractAll() {
      var luigisMansionRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "luigis_mansion.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(luigisMansionRom);
    }
  }
}