using fin.io;
using fin.util.asserts;

using glo.api;

using uni.platforms.desktop;


namespace uni.games.glover {
  internal class GloverExtractor {
    public void ExtractAll() {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover");
      Asserts.Nonnull(gloverSteamDirectory,
                      "Could not find Glover installed in Steam.");

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory!);

      var topLevelObjectDirectory =
          gloverFileHierarchy.Root.TryToGetSubdir("data/objects");
      if (topLevelObjectDirectory == null) {
        return;
      }

      foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
        this.ExtractFromDirectory(gloverSteamDirectory!, objectDirectory);
      }
    }

    private void ExtractFromDirectory(IDirectory gloverSteamDirectory,
                                      IFileHierarchyDirectory objectDirectory) {
      var objectFiles = objectDirectory.FilesWithExtension(".glo");

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(objectDirectory);
      var textureDirectories = gloverSteamDirectory
                               .GetSubdir("data/textures/generic")
                               .GetExistingSubdirs()
                               .ToList();

      try {
        var levelTextureDirectory = gloverSteamDirectory.GetSubdir(
            objectDirectory.LocalPath.Replace("data\\objects",
                                              "data\\textures"));
        textureDirectories.Add(levelTextureDirectory);
        textureDirectories.AddRange(levelTextureDirectory.GetExistingSubdirs());
      } catch { }

      foreach (var objectFile in objectFiles) {
        var outputDirectory =
            parentOutputDirectory.GetSubdir(objectFile.NameWithoutExtension,
                                            true);
        new ManualGloApi().Run(outputDirectory, textureDirectories,
                               objectFile.Impl, 30);
      }
    }
  }
}