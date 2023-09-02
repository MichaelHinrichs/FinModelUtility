using fin.io.bundles;

using uni.config;
using uni.games.battalion_wars_1;
using uni.games.battalion_wars_2;
using uni.games.chibi_robo;
using uni.games.dead_space_1;
using uni.games.dead_space_2;
using uni.games.dead_space_3;
using uni.games.doshin_the_giant;
using uni.games.glover;
using uni.games.great_ace_attorney;
using uni.games.halo_wars;
using uni.games.luigis_mansion_3d;
using uni.games.majoras_mask_3d;
using uni.games.mario_kart_double_dash;
using uni.games.midnight_club_2;
using uni.games.nintendogs_labrador_and_friends;
using uni.games.ocarina_of_time;
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
  public class RootFileGatherer {
    public IFileBundleDirectory GatherAllFiles() {
      IFileBundleGathererAccumulator accumulator =
          Config.Instance.ExtractorSettings.UseMultithreadingToExtractRoms
              ? new ParallelFileBundleGathererAccumulator()
              : new FileBundleGathererAccumulator();

      var gatherers = new IFileBundleGatherer[] {
          new BattalionWars1FileGatherer(),
          new BattalionWars2FileGatherer(),
          new ChibiRoboFileGatherer(),
          new DeadSpace1FileGatherer(),
          new DeadSpace2FileGatherer(),
          new DeadSpace3FileGatherer(),
          new DoshinTheGiantFileGatherer(),
          new GloverModelFileGatherer(),
          new GreatAceAttorneyModelFileGatherer(),
          new HaloWarsModelFileGatherer(),
          new LuigisMansion3dModelFileGatherer(),
          new MajorasMask3dFileGatherer(),
          new MarioKartDoubleDashFileGatherer(),
          new MidnightClub2FileGatherer(),
          new NintendogsLabradorAndFriendsFileBundleGatherer(),
          new OcarinaOfTimeFileBundleGatherer(),
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
          accumulator.GatherFileBundles());
    }
  }
}