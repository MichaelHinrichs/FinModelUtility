using uni.platforms;
using fin.io.bundles;
using sm64.api;


namespace uni.games.super_mario_64 {
  public class SuperMario64FileGatherer :
      IFileBundleGatherer<Sm64LevelFileBundle> {
    public IFileBundleDirectory<Sm64LevelFileBundle>? GatherFileBundles(
        bool assert) {
      var superMario64Rom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "super_mario_64.z64", assert);
      if (superMario64Rom == null) {
        return null;
      }

      var rootNode =
          new FileBundleDirectory<Sm64LevelFileBundle>("super_mario_64");

      var levelIds = Enum.GetValues<LevelId>().ToList();
      levelIds.Sort((lhs, rhs) => lhs.ToString().CompareTo(rhs.ToString()));
      foreach (var levelId in levelIds) {
        rootNode.AddFileBundle(
            new Sm64LevelModelFileBundle(superMario64Rom, levelId));
        rootNode.AddFileBundle(
            new Sm64LevelSceneFileBundle(superMario64Rom, levelId));
      }

      return rootNode;
    }
  }
}