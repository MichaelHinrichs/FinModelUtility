using uni.platforms.desktop;
using glo.api;
using uni.util.io;
using fin.util.asserts;

namespace uni.games.glover {
  internal class GloverExtractor {
    public void ExtractAll() {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover");
      Asserts.Nonnull(gloverSteamDirectory, "Could not find Glover installed in Steam.");

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory!);

      var topLevelObjectDirectory = gloverFileHierarchy.Root.TryToGetSubdir("data/objects");
      if (topLevelObjectDirectory != null) {
        foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
          var objectFiles = objectDirectory.FilesWithExtension(".glo");
          foreach (var objectFile in objectFiles) {
            var outputDirectory = GameFileHierarchyUtil.GetOutputDirectoryForDirectory(objectDirectory).GetSubdir(objectFile.NameWithoutExtension, true);
            new ManualGloApi().Run(outputDirectory, objectFile.Impl, 30);
          }
        }
      }
    }
  }
}