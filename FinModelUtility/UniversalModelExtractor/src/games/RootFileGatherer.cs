using fin.model;

using uni.games.glover;


namespace uni.games {
  public class RootModelFileGatherer {
    public RootModelDirectory GatherAllModelFiles() {
      var rootModelDirectory = new RootModelDirectory();

      rootModelDirectory.AddSubdirIfNotNull(
          new GloverModelFileGatherer().GatherModelFileBundles(false));

      return rootModelDirectory;
    }
  }
}