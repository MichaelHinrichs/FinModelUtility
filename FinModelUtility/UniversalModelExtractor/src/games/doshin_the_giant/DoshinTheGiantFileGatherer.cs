using fin.io.bundles;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.doshin_the_giant {
  public class DoshinTheGiantFileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "doshin_the_giant.gcm", out var doshinTheGiantRom)) {
        yield break;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              doshinTheGiantRom);

      yield break;
    }
  }
}