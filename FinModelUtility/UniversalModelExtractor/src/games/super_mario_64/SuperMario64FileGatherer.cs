using fin.io;

using uni.platforms;

using fin.io.bundles;

using sm64.api;


namespace uni.games.super_mario_64 {
  public class SuperMario64FileGatherer :
      IFileBundleGatherer<Sm64LevelFileBundle> {
    public IEnumerable<Sm64LevelFileBundle> GatherFileBundles(
        bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "super_mario_64.z64",
              assert,
              out var superMario64Rom)) {
        yield break;
      }

      var superMario64Directory =
          DirectoryConstants.ROMS_DIRECTORY.GetSubdir("super_mario_64", true);
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