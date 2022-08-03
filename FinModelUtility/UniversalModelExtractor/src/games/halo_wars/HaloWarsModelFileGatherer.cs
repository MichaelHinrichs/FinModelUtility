using fin.model;

using hw.api;

using uni.platforms.desktop;


namespace uni.games.halo_wars {
  public class HaloWarsModelFileGatherer : IModelFileGatherer<
      IHaloWarsModelFileBundle> {
    public IModelDirectory<IHaloWarsModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var haloWarsSteamDirectory = SteamUtils.GetGameDirectory("HaloWarsDE", assert);
      if (haloWarsSteamDirectory == null) {
        return null;
      }

      return null;
    }
  }
}