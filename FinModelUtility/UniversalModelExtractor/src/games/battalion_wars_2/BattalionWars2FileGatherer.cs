using fin.io;
using fin.model;

using modl.api;

using uni.games.battalion_wars_1;
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

      foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles =
            directory.Files.Where(file => file.Name.EndsWith(".res.gz"));
        foreach (var resFile in resFiles) {
          didUpdate |= new ResDump().Run(resFile);
        }

        if (didUpdate) {
          directory.Refresh();
        }
      }

      return new FileHierarchyBundler<ModlModelFileBundle>(
          directory => {
            var modlFiles = directory.FilesWithExtension(".modl");
            var animFiles = directory.FilesWithExtension(".anim");

            var svetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "SVET");

            var gruntModlFiles =
                modlFiles.Where(modlFile =>
                                    modlFile.Name.EndsWith("G_HI_LOD.modl"));
            var vetModlFiles =
                modlFiles.Where(modlFile =>
                                    modlFile.Name.EndsWith("V_HI_LOD.modl"));

            var fvAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith("FV"))
                         .ToArray();
            var wgruntAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "WG"))
                         .ToArray();

            var otherModlFiles =
                modlFiles.Where(
                             modlFile =>
                                 !svetModlFile.Contains(modlFile) &&
                                 !gruntModlFiles.Contains(modlFile) &&
                                 !vetModlFiles.Contains(modlFile)
                         )
                         .ToArray();

            var allModlsAndAnims =
                new (IEnumerable<IFileHierarchyFile>, IList<IFileHierarchyFile>?
                    )
                    [] {
                        (svetModlFile, fvAnimFiles),
                        (gruntModlFiles, wgruntAnimFiles),
                        (vetModlFiles, fvAnimFiles),
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
                                        ModlType = ModlType.BW2,
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