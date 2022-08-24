using System.IO.Compression;

using fin.io;
using fin.model;

using modl.api;

using uni.platforms;
using uni.platforms.gcn;
using uni.util.io;


namespace uni.games.battalion_wars_1 {
  public class
      BattalionWars1FileGatherer : IModelFileGatherer<ModlModelFileBundle> {
    public IModelDirectory<ModlModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var battalionWarsRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "battalion_wars_1.gcm");

      if (battalionWarsRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              battalionWarsRom);

      foreach (var directory in fileHierarchy) {
        var didUpdate = false;
        var resFiles = directory.FilesWithExtension(".res.gz");
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
            var sgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "SGRUNT");

            var tvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "TVET");
            var tgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "TGRUNT");

            var uvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "UVET");
            var ugruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "UGRUNT");

            var wvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "WVET");
            var wgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "WGRUNT");

            var xvetModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "XVET");
            var xgruntModlFile =
                modlFiles.Where(modlFile =>
                                    modlFile.NameWithoutExtension is "XGRUNT");


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

            var sgAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "SG"))
                         .ToArray();
            var uvAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "UV"))
                         .ToArray();
            var wgruntAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "WGRUNT"))
                         .ToArray();
            var xgAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "XG"))
                         .ToArray();
            var xvAnimFiles =
                animFiles.Where(
                             animFile =>
                                 animFile.NameWithoutExtension.StartsWith(
                                     "XV"))
                         .ToArray();

            var otherModlFiles =
                modlFiles.Where(
                             modlFile =>
                                 !sgruntModlFile.Contains(modlFile) &&
                                 !svetModlFile.Contains(modlFile) &&
                                 !tgruntModlFile.Contains(modlFile) &&
                                 !tvetModlFile.Contains(modlFile) &&
                                 !ugruntModlFile.Contains(modlFile) &&
                                 !uvetModlFile.Contains(modlFile) &&
                                 !wgruntModlFile.Contains(modlFile) &&
                                 !wvetModlFile.Contains(modlFile) &&
                                 !xgruntModlFile.Contains(modlFile) &&
                                 !xvetModlFile.Contains(modlFile)
                         )
                         .ToArray();

            var allModlsAndAnims =
                new (IEnumerable<IFileHierarchyFile>, IList<IFileHierarchyFile>?
                    )
                    [] {
                        (sgruntModlFile,
                         fgAnimFiles.Concat(sgAnimFiles).ToArray()),
                        (svetModlFile, fvAnimFiles),
                        (tgruntModlFile, fgAnimFiles),
                        (tvetModlFile, fvAnimFiles),
                        (ugruntModlFile, fgAnimFiles),
                        (uvetModlFile,
                         fvAnimFiles.Concat(uvAnimFiles).ToArray()),
                        (wgruntModlFile,
                         fgAnimFiles.Concat(wgruntAnimFiles).ToArray()),
                        (wvetModlFile, fvAnimFiles),
                        (xgruntModlFile,
                         fgAnimFiles.Concat(xgAnimFiles).ToArray()),
                        (xvetModlFile,
                         fvAnimFiles.Concat(xvAnimFiles).ToArray()),
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
                                        ModlType = ModlType.BW1,
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