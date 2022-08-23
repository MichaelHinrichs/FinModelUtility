using fin.model;

using modl.api;

using uni.platforms;
using uni.platforms.wii;
using uni.util.io;


namespace uni.games.battalion_wars_2 {
  public class
      BattalionWars2FileGatherer : IModelFileGatherer<ModlModelFileBundle> {
    public IModelDirectory<ModlModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var battalionWarsRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "battalion_wars_2.iso");

      if (battalionWarsRom == null) {
        return null;
      }

      var fileHierarchy =
          new WiiFileHierarchyExtractor().ExtractFromRom(
              battalionWarsRom);

      /*foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles = directory.FilesWithExtension(".res");
        foreach (var resFile in resFiles) {
          didUpdate |= new ResDump().Run(resFile);
        }

        if (didUpdate) {
          directory.Refresh();
        }
      }*/

      return null;

      return new FileHierarchyBundler<ModlModelFileBundle>(
          directory => {
            var modlFiles = directory.FilesWithExtension(".modl");
            var animFiles = directory.FilesWithExtension(".anim");

            var tvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "TVET");
            var tgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "TGRUNT");
            var wvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "WVET");
            var wgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "WGRUNT");

            var fvAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith("FV"))
                         .ToArray();
            var fgAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith("FG"))
                         .ToArray();
            var wgruntAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "WGRUNT"))
                         .ToArray();

            var otherModlFiles =
                modlFiles.Where(
                             modlFile =>
                                 !tvetModlFile.Contains(modlFile) &&
                                 !wvetModlFile.Contains(modlFile) &&
                                 !tgruntModlFile.Contains(modlFile) &&
                                 !wgruntModlFile.Contains(modlFile))
                         .ToArray();

            var allModlsAndAnims = new[] {
                (tvetModlFile, fvAnimFiles),
                (wvetModlFile, fvAnimFiles),
                (tgruntModlFile, fgAnimFiles),
                (wgruntModlFile, fgAnimFiles.Concat(wgruntAnimFiles).ToArray()),
                (otherModlFiles, null),
            };

            var bundles =
                allModlsAndAnims
                    .SelectMany(
                        modlsAndAnims =>
                            modlsAndAnims
                                .Item1
                                .Select(
                                    modlFile => new ModlModelFileBundle {
                                        ModlFile = modlFile,
                                        AnimFiles = modlsAndAnims.Item2
                                    }))
                    .ToList();
            bundles.Sort((lhs, rhs) =>
                             lhs.MainFile.Name.CompareTo(rhs.MainFile.Name));

            return bundles;
          }
      ).GatherBundles(fileHierarchy);
    }
  }
}