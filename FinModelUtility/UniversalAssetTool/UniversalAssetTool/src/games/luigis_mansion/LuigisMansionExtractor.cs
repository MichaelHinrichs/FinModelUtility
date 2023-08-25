using fin.log;

using uni.platforms.gcn;

namespace uni.games.luigis_mansion {
  public class LuigisMansionExtractor : IExtractor {
    private readonly ILogger logger_ =
        Logging.Create<LuigisMansionExtractor>();

    public void ExtractAll() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "luigis_mansion",
              GcnFileHierarchyExtractor.Options
                                       .Standard()
                                       .UseRarcDumpForExtensions(
                                           // For some reason, some MDL files are compressed as RARC.
                                           ".mdl"),
              out var fileHierarchy)) {
        return;
      }

      // TODO: Use Mdl2Fbx
    }
  }
}