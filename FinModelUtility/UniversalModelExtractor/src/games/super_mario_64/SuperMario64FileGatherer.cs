using uni.platforms;
using uni.platforms.threeDs;
using uni.util.io;
using cmb.api;
using fin.io.bundles;
using sm64.api;


namespace uni.games.super_mario_64 {
  public class SuperMario64FileGatherer
      : IFileBundleGatherer<Sm64LevelModelFileBundle> {
    public IFileBundleDirectory<Sm64LevelModelFileBundle>? GatherFileBundles(
        bool assert) {
      var superMario64Rom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "super_mario_64.z64", assert);
      if (superMario64Rom == null) {
        return null;
      }

      var rootNode =
          new FileBundleDirectory<Sm64LevelModelFileBundle>("super_mario_64");

      foreach (var levelId in Enum.GetValues<LevelId>()) {
        rootNode.AddFileBundle(
            new Sm64LevelModelFileBundle(superMario64Rom, levelId));
      }

      return rootNode;
    }
  }
}