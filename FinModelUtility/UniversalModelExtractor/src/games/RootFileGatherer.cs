using fin.io.bundles;

using uni.config;
using uni.games.battalion_wars_1;
using uni.games.battalion_wars_2;
using uni.games.glover;
using uni.games.great_ace_attorney;
using uni.games.halo_wars;
using uni.games.luigis_mansion_3d;
using uni.games.majoras_mask_3d;
using uni.games.mario_kart_double_dash;
using uni.games.midnight_club_2;
using uni.games.ocarina_of_time_3d;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.professor_layton_vs_phoenix_wright;
using uni.games.super_mario_64;
using uni.games.super_mario_sunshine;
using uni.games.super_smash_bros_melee;
using uni.games.wind_waker;
using uni.util.io;


namespace uni.games {
  public class RootModelFileGatherer {
    public IFileBundleDirectory GatherAllModelFiles() {
      IFileBundleGathererAccumulator accumulator =
          Config.Instance.UseMultithreadingToExtractRoms
              ? new ParallelFileBundleGathererAccumulator()
              : new FileBundleGathererAccumulator();

      var gatherers = new IFileBundleGatherer[] {
          new BattalionWars1FileGatherer(),
          new BattalionWars2FileGatherer(),
          new GloverModelFileGatherer(),
          new GreatAceAttorneyModelFileGatherer(),
          new HaloWarsModelFileGatherer(),
          new LuigisMansion3dModelFileGatherer(),
          new MajorasMask3dFileGatherer(),
          new MarioKartDoubleDashFileGatherer(),
          new MidnightClub2FileGatherer(),
          new OcarinaOfTime3dFileGatherer(),
          new Pikmin1ModelFileGatherer(),
          new Pikmin2FileGatherer(),
          new ProfessorLaytonVsPhoenixWrightModelFileGatherer(),
          new SuperMario64FileGatherer(),
          new SuperMarioSunshineModelFileGatherer(),
          new SuperSmashBrosMeleeModelFileGatherer(),
          new WindWakerFileGatherer(),
      };
      foreach (var gatherer in gatherers) {
        accumulator.Add(gatherer);
      }

      return new FileBundleHierarchyOrganizer().Organize(
          accumulator.GatherFileBundles(false));
    }
  }
}