using fin.audio;
using fin.io;
using fin.io.bundles;
using glo.api;
using uni.platforms.desktop;


namespace uni.games.glover {
  public class GloverModelFileGatherer
      : IFileBundleGatherer<IFileBundle> {
    public IFileBundleDirectory<IFileBundle>? GatherFileBundles(
        bool assert) {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover", assert);
      if (gloverSteamDirectory == null) {
        return null;
      }

      var rootModelDirectory =
          new FileBundleDirectory<IFileBundle>("glover");

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory);

      var dataDirectory = gloverFileHierarchy.Root.TryToGetSubdir("data");
      var parentDataDirectory = rootModelDirectory.AddSubdir("data");

      var parentBgmDirectory = parentDataDirectory.AddSubdir("bgm");
      var topLevelBgmDirectory = dataDirectory.TryToGetSubdir("bgm");
      foreach (var bgmFile in topLevelBgmDirectory.Files) {
        parentBgmDirectory.AddFileBundle(new OggAudioFileBundle(bgmFile));
      }

      var parentObjectDirectory = parentDataDirectory.AddSubdir("objects");
      var topLevelObjectDirectory = dataDirectory.TryToGetSubdir("objects");
      foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
        this.AddObjectDirectory_(
            parentObjectDirectory.AddSubdir(objectDirectory.Name),
            gloverFileHierarchy, objectDirectory);
      }
      return rootModelDirectory;
    }

    private void AddObjectDirectory_(
        IFileBundleDirectory<IFileBundle> parentNode,
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
      } catch {
        // ignored
      }

      foreach (var objectFile in objectFiles) {
        parentNode.AddFileBundle(
            new GloModelFileBundle(objectFile, textureDirectories));
      }
    }
  }
}