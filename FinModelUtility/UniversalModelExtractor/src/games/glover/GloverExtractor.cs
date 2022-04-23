using uni.platforms.desktop;
using glo.api;
using uni.util.io;
using fin.util.asserts;
using System.Collections.Generic;
using fin.io;

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

          var parentOutputDirectory = GameFileHierarchyUtil.GetOutputDirectoryForDirectory(objectDirectory);
          var textureDirectories = new List<IDirectory>();
          foreach (var genericTextureDirectory in gloverSteamDirectory.GetSubdir("data/textures/generic").GetExistingSubdirs()) {
            textureDirectories.Add(genericTextureDirectory);
          }
          
          try {
            var levelTextureDirectory = gloverSteamDirectory.GetSubdir(objectDirectory.LocalPath.Replace("data\\objects", "data\\textures"));
            textureDirectories.Add(levelTextureDirectory);
            foreach (var subdir in levelTextureDirectory.GetExistingSubdirs()) {
              textureDirectories.Add(subdir);
            }
          } catch { }

          foreach (var objectFile in objectFiles) {
            var outputDirectory = parentOutputDirectory.GetSubdir(objectFile.NameWithoutExtension, true);
            new ManualGloApi().Run(outputDirectory, textureDirectories, objectFile.Impl, 30);
          }
        }
      }
    }
  }
}