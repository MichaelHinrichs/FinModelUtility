using fin.log;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.battalion_wars {
  public class BattalionWarsExtractor {
    private readonly ILogger logger_ =
        Logging.Create<BattalionWarsExtractor>();

    public void ExtractAll() {
      var battalionWarsRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "battalion_wars.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              battalionWarsRom);

      // TODO: Need to extract from .res files

      // TODO: Use Modl2Fbx
    }
  }
}