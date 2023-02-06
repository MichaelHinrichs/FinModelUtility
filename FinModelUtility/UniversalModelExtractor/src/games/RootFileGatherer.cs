using System.Runtime.CompilerServices;

using fin.io.bundles;

using Microsoft.Toolkit.HighPerformance.Helpers;

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


namespace uni.games {
  public class RootModelFileGatherer {
    public RootFileBundleDirectory GatherAllModelFiles() {
      var rootFileBundleDirectory = new RootFileBundleDirectory();

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
      };

      var useMultiThreading = true;
      if (useMultiThreading) {
        var results = new IFileBundleDirectory?[gatherers.Length];
        ParallelHelper.For(0,
                           gatherers.Length,
                           new GathererRunner(gatherers, results));
        foreach (var result in results) {
          rootFileBundleDirectory.AddSubdirIfNotNull(result);
        }
      } else {
        foreach (var gatherer in gatherers) {
          rootFileBundleDirectory.AddSubdirIfNotNull(gatherer
              .GatherFileBundles(false));
        }
      }

      rootFileBundleDirectory.RemoveEmptyChildren();

      return rootFileBundleDirectory;
    }

    private readonly struct GathererRunner : IAction {
      private readonly IReadOnlyList<IFileBundleGatherer> gatherers_;
      private readonly IList<IFileBundleDirectory?> results_;


      public GathererRunner(IReadOnlyList<IFileBundleGatherer> gatherers,
                            IList<IFileBundleDirectory?> results) {
        this.gatherers_ = gatherers;
        this.results_ = results;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Invoke(int i) {
        try {
          this.results_[i] = this.gatherers_[i].GatherFileBundles(false);
        } catch (Exception e) {
          ;
        }
      }
    }
  }
}