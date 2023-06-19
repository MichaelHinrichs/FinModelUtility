using fin.io.bundles;

using mod.cli;

using uni.platforms.gcn;


namespace uni.games.pikmin_1 {
  public class Pikmin1ModelFileGatherer
      : IFileBundleGatherer<ModModelFileBundle> {
    public IEnumerable<ModModelFileBundle> GatherFileBundles(bool assert) {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "pikmin_1",
              GcnFileHierarchyExtractor.Options.Empty(),
              out var fileHierarchy)) {
        return Enumerable.Empty<ModModelFileBundle>();
      }

      return fileHierarchy.SelectMany(directory => {
        // TODO: Handle special cases:
        // - olimar
        // - pikmin
        // - frog

        var anmFiles = directory.FilesWithExtension(".anm").ToArray();
        return directory
               .FilesWithExtension(".mod")
               .Select(modFile => {
                 var anmFile = anmFiles
                     .FirstOrDefault(
                         anmFile => anmFile.NameWithoutExtension ==
                                    modFile.NameWithoutExtension);
                 return new ModModelFileBundle {
                     GameName = "pikmin_1",
                     ModFile = modFile,
                     AnmFile = anmFile,
                 };
               });
      });
    }
  }
}