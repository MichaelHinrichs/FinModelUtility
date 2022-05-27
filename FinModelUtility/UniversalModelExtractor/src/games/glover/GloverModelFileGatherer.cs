using fin.io;
using fin.model;

using glo.api;

using uni.platforms.desktop;


namespace uni.games.glover {
  public class
      GloverModelFileGatherer : IModelFileGatherer<GloModelFileBundle> {
    public IModelDirectory<GloModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover", assert);
      if (gloverSteamDirectory == null) {
        return null;
      }

      var rootModelDirectory = new ModelDirectory<GloModelFileBundle>("glover");
      var parentObjectDirectory = rootModelDirectory.AddSubdir("data")
                                                    .AddSubdir("objects");

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory);
      var topLevelObjectDirectory =
          gloverFileHierarchy.Root.TryToGetSubdir("data/objects");
      foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
        this.AddObjectDirectory(
            parentObjectDirectory.AddSubdir(objectDirectory.Name),
            gloverFileHierarchy, objectDirectory);
      }
      return rootModelDirectory;
    }

    private void AddObjectDirectory(
        IModelDirectory<GloModelFileBundle> parentNode,
        IFileHierarchy gloverFileHierarchy,
        IFileHierarchyDirectory objectDirectory) {
      var objectFiles = objectDirectory.FilesWithExtension(".glo");

      var gloverSteamDirectory = gloverFileHierarchy.Root;
      var textureDirectories = gloverSteamDirectory
                               .TryToGetSubdir("data/textures/generic")
                               .Subdirs.ToList();

      try {
        var levelTextureDirectory = gloverSteamDirectory.TryToGetSubdir(
            objectDirectory.LocalPath.Replace("data\\objects",
                                              "data\\textures"));
        textureDirectories.Add(levelTextureDirectory);
        textureDirectories.AddRange(levelTextureDirectory.Subdirs);
      } catch(Exception e) {
        ;
      }

      foreach (var objectFile in objectFiles) {
        parentNode.AddFileBundle(
            new GloModelFileBundle(objectFile, textureDirectories));
      }
    }
  }
}