using fin.io.bundles;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.chibi_robo {
  public class ChibiRoboFileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "chibi_robo.gcm", out var chibiRoboRom)) {
        yield break;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              chibiRoboRom);

      yield break;
    }
  }
}