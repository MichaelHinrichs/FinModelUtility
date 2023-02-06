using CommandLine;

using uni.games;
using uni.games.animal_crossing;
using uni.games.battalion_wars_1;
using uni.games.battalion_wars_2;
using uni.games.glover;
using uni.games.halo_wars;
using uni.games.luigis_mansion;
using uni.games.luigis_mansion_3d;
using uni.games.mario_kart_double_dash;
using uni.games.midnight_club_2;
using uni.games.ocarina_of_time;
using uni.games.ocarina_of_time_3d;
using uni.games.paper_mario_the_thousand_year_door;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.professor_layton_vs_phoenix_wright;
using uni.games.super_mario_sunshine;
using uni.games.wind_waker;

namespace uni.cli {
  public interface IExtractorOptions {
    IExtractor CreateExtractor();
  }

  public interface IExtractorOptions<TExtractor> : IExtractorOptions
      where TExtractor : IExtractor, new() {
    IExtractor IExtractorOptions.CreateExtractor() => this.CreateExtractor();
    TExtractor CreateExtractor() => new();
  }


  [Verb("animal_crossing",
        HelpText = "Extract models from Animal Crossing.")]
  public class AnimalCrossingOptions
      : IExtractorOptions<AnimalCrossingExtractor> { }

  [Verb("battalion_wars_1",
        HelpText = "Extract models from Battalion Wars 1.")]
  public class BattalionWars1Options 
      : IExtractorOptions<BattalionWars1Extractor> { }

  [Verb("battalion_wars_2",
        HelpText = "Extract models from Battalion Wars 2.")]
  public class BattalionWars2Options
      : IExtractorOptions<BattalionWars2Extractor> { }

  [Verb("halo_wars",
        HelpText = "Extract models from Halo Wars.")]
  public class HaloWarsOptions : IExtractorOptions<HaloWarsExtractor> { }

  [Verb("glover",
        HelpText = "Extract models from Glover.")]
  public class GloverOptions : IExtractorOptions<GloverExtractor> { }

  [Verb("luigis_mansion", HelpText = "Extract models from Luigi's Mansion.")]
  public class LuigisMansionOptions 
      : IExtractorOptions<LuigisMansionExtractor> { }

  [Verb("luigis_mansion_3d",
        HelpText = "Extract models from Luigi's Mansion 3D.")]
  public class LuigisMansion3dOptions
      : IExtractorOptions<LuigisMansion3dExtractor> { }

  [Verb("mario_kart_double_dash",
        HelpText = "Extract models from Mario Kart: Double Dash.")]
  public class MarioKartDoubleDashOptions
      : IExtractorOptions<MarioKartDoubleDashExtractor> { }

  [Verb("midnight_club_2",
        HelpText = "Extract models from Midnight Club 2.")]
  public class MidnightClub2Options
      : IExtractorOptions<MidnightClub2Extractor> { }

  [Verb("ocarina_of_time", HelpText = "Extract models from Ocarina of Time.")]
  public class OcarinaOfTimeOptions
      : IExtractorOptions<OcarinaOfTimeExtractor> { }

  [Verb("ocarina_of_time_3d",
        HelpText = "Extract models from Ocarina of Time 3D.")]
  public class OcarinaOfTime3dOptions
      : IExtractorOptions<OcarinaOfTime3dExtractor> { }

  [Verb("paper_mario_the_thousand_year_door",
        HelpText = "Extract models from Paper Mario: The Thousand Year Door.")]
  public class PaperMarioTheThousandYearDoorOptions
      : IExtractorOptions<PaperMarioTheThousandYearDoorExtractor> { }

  [Verb("pikmin_1", HelpText = "Extract models from Pikmin 1.")]
  public class Pikmin1Options
      : IExtractorOptions<Pikmin1Extractor> { }

  [Verb("pikmin_2", HelpText = "Extract models from Pikmin 2.")]
  public class Pikmin2Options : IExtractorOptions<Pikmin2Extractor> { }

[Verb("professor_layton_vs_phoenix_wright",
        HelpText = "Extract models from Professor Layton vs. Phoenix Wright.")]
  public class ProfessorLaytonVsPhoenixWrightOptions
      : IExtractorOptions<ProfessorLaytonVsPhoenixWrightExtractor> { }

  [Verb("super_mario_sunshine",
        HelpText = "Extract models from Super Mario Sunshine.")]
  public class SuperMarioSunshineOptions
      : IExtractorOptions<SuperMarioSunshineExtractor> { }

  [Verb("wind_waker",
        HelpText = "Extract models from Wind Waker.")]
  public class WindWakerOptions : IExtractorOptions<WindWakerExtractor> { }
}