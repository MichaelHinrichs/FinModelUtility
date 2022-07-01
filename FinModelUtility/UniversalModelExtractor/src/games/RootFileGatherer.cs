using fin.model;

using uni.games.glover;
using uni.games.luigis_mansion_3d;
using uni.games.mario_kart_double_dash;
using uni.games.ocarina_of_time_3d;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.super_mario_sunshine;


namespace uni.games {
  public class RootModelFileGatherer {
    public RootModelDirectory GatherAllModelFiles() {
      var rootModelDirectory = new RootModelDirectory();

      var gatherers = new IModelFileGatherer[] {
          new GloverModelFileGatherer(),
          new LuigisMansion3dModelFileGatherer(),
          new MarioKartDoubleDashFileGatherer(),
          new OcarinaOfTime3dFileGatherer(),
          new Pikmin1ModelFileGatherer(),
          new Pikmin2FileGatherer(),
          new SuperMarioSunshineModelFileGatherer(),
      };

      foreach (var gatherer in gatherers) {
        rootModelDirectory.AddSubdirIfNotNull(
            gatherer.GatherModelFileBundles(false));
      }

      rootModelDirectory.RemoveEmptyChildren();

      return rootModelDirectory;
    }
  }
}