using fin.log;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.luigis_mansion {
  public class LuigisMansionExtractor : IExtractor {
    private readonly ILogger logger_ =
        Logging.Create<LuigisMansionExtractor>();

    public void ExtractAll() {
      var luigisMansionRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "luigis_mansion.gcm");

      var options =
          GcnFileHierarchyExtractor.Options.Standard()
                                   .UseRarcDumpForExtensions(
                                       // For some reason, some MDL files are compressed as RARC.
                                       ".mdl");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              luigisMansionRom);

      // TODO: Use Mdl2Fbx
    }
  }
}