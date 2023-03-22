using System.Collections.Generic;

using fin.io;
using fin.io.bundles;

using visceral.api;

using uni.platforms.desktop;


namespace uni.games.dead_space_1 {
  public class DeadSpace1FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!SteamUtils.TryGetGameDirectory("Dead Space",
                                          out var deadSpaceDir,
                                          assert)) {
        yield break;
      }

      var originalGameFileHierarchy = new FileHierarchy(deadSpaceDir);

      var baseOutputDirectory =
          GameFileHierarchyUtil.GetWorkingDirectoryForDirectory(
              originalGameFileHierarchy.Root,
              "dead_space_1");
      if (!baseOutputDirectory.Exists) {
        var strExtractor = new StrExtractor();
        foreach (var strFile in originalGameFileHierarchy.SelectMany(
                     dir => dir.FilesWithExtensionRecursive(".str"))) {
          strExtractor.Extract(strFile, baseOutputDirectory);
        }
      }

      var assetFileHierarchy = new FileHierarchy(baseOutputDirectory);

      var charsDirectory = assetFileHierarchy.Root.GetExistingSubdir("chars");
      foreach (var charSubdir in charsDirectory.Subdirs) {
        var riggedSubdir = charSubdir.GetExistingSubdir("rigged/export");
        var geoFiles =
            riggedSubdir.Files.Where(file => file.Name.EndsWith(".geo"))
                        .ToArray();

        var cctSubdir = charSubdir.GetExistingSubdir("cct/export");
        var rcbFile =
            cctSubdir.Files.Single(file => file.Name.EndsWith(".rcb.WIN"));

        Tg4ImageFileBundle[]? textureFiles = null;
        if (charSubdir.TryToGetExistingSubdir("rigged/textures",
                                              out var textureDir)) {
          var textureDirFiles = textureDir.Files.ToArray();
          var tg4hFiles =
              textureDirFiles.Where(file => file.Extension == ".tg4h")
                             .ToDictionary(file => file.NameWithoutExtension);
          textureFiles =
              textureDirFiles.Where(file => file.Extension == ".tg4d")
                             .Select(tg4dFile => new Tg4ImageFileBundle {
                                 Tg4dFile = tg4dFile,
                                 Tg4hFile =
                                     tg4hFiles[tg4dFile.NameWithoutExtension]
                             })
                             .ToArray();
        }

        yield return new GeoModelFileBundle {
            GeoFiles = geoFiles,
            RcbFile = rcbFile,
            Tg4ImageFileBundles = textureFiles
        };
      }

      /*return assetFileHierarchy
       .SelectMany(dir => dir.Files.Where(file => file.Name.EndsWith(".rcb.WIN")))
       .Select(
           rcbFile => new GeoModelFileBundle { RcbFile = rcbFile });*/
    }
  }
}