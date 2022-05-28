using fin.model;

using uni.games.glover;
using uni.games.luigis_mansion_3d;
using uni.games.pikmin_1;


namespace uni.games {
  public class RootModelFileGatherer {
    public RootModelDirectory GatherAllModelFiles() {
      var rootModelDirectory = new RootModelDirectory();

      rootModelDirectory.AddSubdirIfNotNull(
          new GloverModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new LuigisMansion3dModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new Pikmin1ModelFileGatherer().GatherModelFileBundles(false));

      rootModelDirectory.RemoveEmptyChildren();

      return rootModelDirectory;
    }
  }
}