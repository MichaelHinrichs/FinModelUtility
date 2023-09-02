using fin.io;
using fin.io.bundles;

using sm64.api;

using uni.platforms;

namespace uni.games.super_mario_64 {
  public class SuperMario64FileGatherer :
      IFileBundleGatherer<Sm64LevelFileBundle> {
    public IEnumerable<Sm64LevelFileBundle> GatherFileBundles() {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "super_mario_64.z64",
              out var superMario64Rom)) {
        yield break;
      }

      var superMario64Directory =
          DirectoryConstants.ROMS_DIRECTORY.GetOrCreateSubdir("super_mario_64");
      var fileHierarchy = new FileHierarchy(superMario64Directory);
      var root = fileHierarchy.Root;

      var levelIds = Enum.GetValues<LevelId>().ToList();
      levelIds.Sort((lhs, rhs) => lhs.ToString().CompareTo(rhs.ToString()));
      foreach (var levelId in levelIds) {
        yield return new Sm64LevelSceneFileBundle(root, superMario64Rom, levelId);
      }
    }
  }
}