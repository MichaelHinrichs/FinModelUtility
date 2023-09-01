using fin.audio.ogg;
using fin.io;
using fin.io.bundles;

using glo.api;

using uni.platforms.desktop;

namespace uni.games.glover {
  public class GloverModelFileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(
        bool assert) {
      if (!SteamUtils.TryGetGameDirectory("Glover",
                                          out var gloverSteamDirectory,
                                          assert)) {
        yield break;
      }

      var gloverFileHierarchy = new FileHierarchy(gloverSteamDirectory);

      var dataDirectory = gloverFileHierarchy.Root.GetExistingSubdir("data");
      var topLevelBgmDirectory = dataDirectory.GetExistingSubdir("bgm");
      foreach (var bgmFile in topLevelBgmDirectory.Files) {
        yield return new OggAudioFileBundle(bgmFile);
      }

      var topLevelObjectDirectory = dataDirectory.GetExistingSubdir("objects");
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
                               .GetExistingSubdir("data/textures/generic")
                               .Subdirs.ToList();

      try {
        var levelTextureDirectory = gloverSteamDirectory.GetExistingSubdir(
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