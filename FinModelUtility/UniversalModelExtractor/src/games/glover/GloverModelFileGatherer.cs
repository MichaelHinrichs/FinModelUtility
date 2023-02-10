using fin.audio;
using fin.io;
using fin.io.bundles;

using glo.api;

using uni.platforms.desktop;


namespace uni.games.glover {
  public class GloverModelFileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(
        bool assert) {
      var gloverSteamDirectory = SteamUtils.GetGameDirectory("Glover", assert);
      if (gloverSteamDirectory == null) {
        yield break;
      }

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory);

      var dataDirectory = gloverFileHierarchy.Root.TryToGetSubdir("data");
      var topLevelBgmDirectory = dataDirectory.TryToGetSubdir("bgm");
      foreach (var bgmFile in topLevelBgmDirectory.Files) {
        yield return new OggAudioFileBundle(bgmFile);
      }

      var topLevelObjectDirectory = dataDirectory.TryToGetSubdir("objects");
      foreach (var objectDirectory in topLevelObjectDirectory.Subdirs) {
        foreach (var fileBundle in this.AddObjectDirectory_(
                     gloverFileHierarchy,
                     objectDirectory)) {
          yield return fileBundle;
        }
      }
    }

    private IEnumerable<IFileBundle> AddObjectDirectory_(
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
        yield return new GloModelFileBundle(objectFile, textureDirectories);
      }
    }
  }
}