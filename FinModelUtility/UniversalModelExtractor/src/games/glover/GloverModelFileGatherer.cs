using fin.io;
using fin.model;

using glo.api;

using uni.platforms.desktop;


namespace uni.games.glover {
  internal class
      GloverModelFileGatherer : IModelFileGatherer<GloModelFileBundle> {
    public IModelDirectory<GloModelFileBundle>? GatherModelFileBundles() {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover");
      if (gloverSteamDirectory == null) {
        return null;
      }

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory);

      var topLevelObjectDirectory =
          gloverFileHierarchy.Root.TryToGetSubdir("data/objects");
      if (topLevelObjectDirectory == null) {
        return null;
      }

      var rootModelDirectory = new ModelDirectory<GloModelFileBundle>("glover");

      var parentObjectDirectory = rootModelDirectory.AddSubdir("data")
                                                    .AddSubdir("objects");
      foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
        this.AddObjectDirectory(
            parentObjectDirectory.AddSubdir(objectDirectory.Name),
            gloverSteamDirectory, objectDirectory);
      }
      return rootModelDirectory;
    }

    private void AddObjectDirectory(
        IModelDirectory<GloModelFileBundle> parentNode,
        IDirectory gloverSteamDirectory,
        IFileHierarchyDirectory objectDirectory) {
      var objectFiles = objectDirectory.FilesWithExtension(".glo");

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
        parentNode.AddFileBundle(
            new GloModelFileBundle(objectFile, textureDirectories));
      }
    }
  }
}