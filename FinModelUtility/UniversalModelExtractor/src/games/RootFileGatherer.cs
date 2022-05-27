using fin.model;

using uni.games.glover;
using uni.games.luigis_mansion_3d;


namespace uni.games {
  public class RootModelFileGatherer {
    public RootModelDirectory GatherAllModelFiles() {
      var rootModelDirectory = new RootModelDirectory();

      rootModelDirectory.AddSubdirIfNotNull(
          new GloverModelFileGatherer().GatherModelFileBundles(false));
      rootModelDirectory.AddSubdirIfNotNull(
          new LuigisMansion3dModelFileGatherer().GatherModelFileBundles(false));

      return rootModelDirectory;
    }
  }
}