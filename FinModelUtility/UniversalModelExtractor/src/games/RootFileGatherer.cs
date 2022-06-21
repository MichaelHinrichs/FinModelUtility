using fin.model;

using uni.games.glover;
using uni.games.luigis_mansion_3d;
using uni.games.ocarina_of_time_3d;
using uni.games.pikmin_1;
using uni.games.pikmin_2;


namespace uni.games {
  public class RootModelFileGatherer {
    public RootModelDirectory GatherAllModelFiles() {
      var rootModelDirectory = new RootModelDirectory();

      rootModelDirectory.AddSubdirIfNotNull(
          new GloverModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new LuigisMansion3dModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new OcarinaOfTime3dFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new Pikmin1ModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new Pikmin2FileGatherer().GatherModelFileBundles(false));

      rootModelDirectory.RemoveEmptyChildren();

      return rootModelDirectory;
    }
  }
}