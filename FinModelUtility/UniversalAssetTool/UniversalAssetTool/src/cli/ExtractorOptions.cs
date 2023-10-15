using CommandLine;

using uni.games;
using uni.games.animal_crossing;
using uni.games.battalion_wars_1;
using uni.games.battalion_wars_2;
using uni.games.glover;
using uni.games.halo_wars;
using uni.games.luigis_mansion;
using uni.games.luigis_mansion_3d;
using uni.games.majoras_mask_3d;
using uni.games.mario_kart_double_dash;
using uni.games.midnight_club_2;
using uni.games.ocarina_of_time_3d;
using uni.games.paper_mario_the_thousand_year_door;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.professor_layton_vs_phoenix_wright;
using uni.games.super_mario_sunshine;
using uni.games.wind_waker;

namespace uni.cli {
  public interface IMassExporterOptions {
    IMassExporter CreateMassExporter();
  }

  public interface IMassExporterOptions<out TMassExporter> : IMassExporterOptions
      where TMassExporter : IMassExporter, new() {
    IMassExporter IMassExporterOptions.CreateMassExporter() => this.CreateMassExporter();
    new TMassExporter CreateMassExporter() => new();
  }


  [Verb("animal_crossing",
        HelpText = "Extract models from Animal Crossing.")]
  public class AnimalCrossingOptions
      : IMassExporterOptions<AnimalCrossingMassExporter> { }

  [Verb("battalion_wars_1",
        HelpText = "Extract models from Battalion Wars 1.")]
  public class BattalionWars1Options 
      : IMassExporterOptions<BattalionWars1MassExporter> { }

  [Verb("battalion_wars_2",
        HelpText = "Extract models from Battalion Wars 2.")]
  public class BattalionWars2Options
      : IMassExporterOptions<BattalionWars2MassExporter> { }

  [Verb("halo_wars",
        HelpText = "Extract models from Halo Wars.")]
  public class HaloWarsOptions : IMassExporterOptions<HaloWarsMassExporter> { }

  [Verb("glover",
        HelpText = "Extract models from Glover.")]
  public class GloverOptions : IMassExporterOptions<GloverMassExporter> { }

  [Verb("luigis_mansion", HelpText = "Extract models from Luigi's Mansion.")]
  public class LuigisMansionOptions 
      : IMassExporterOptions<LuigisMansionMassExporter> { }

  [Verb("luigis_mansion_3d",
        HelpText = "Extract models from Luigi's Mansion 3D.")]
  public class LuigisMansion3dOptions
      : IMassExporterOptions<LuigisMansion3dMassExporter> { }

  [Verb("majoras_mask_3d",
        HelpText = "Extract models from Majora's Mask 3D.")]
  public class MajorasMask3dOptions
      : IMassExporterOptions<MajorasMask3dMassExporter> { }

  [Verb("mario_kart_double_dash",
        HelpText = "Extract models from Mario Kart: Double Dash.")]
  public class MarioKartDoubleDashOptions
      : IMassExporterOptions<MarioKartDoubleDashMassExporter> { }


  [Verb("midnight_club_2",
        HelpText = "Extract models from Midnight Club 2.")]
  public class MidnightClub2Options
      : IMassExporterOptions<MidnightClub2MassExporter> { }

  [Verb("ocarina_of_time_3d",
        HelpText = "Extract models from Ocarina of Time 3D.")]
  public class OcarinaOfTime3dOptions
      : IMassExporterOptions<OcarinaOfTime3dMassExporter> { }

  [Verb("paper_mario_the_thousand_year_door",
        HelpText = "Extract models from Paper Mario: The Thousand Year Door.")]
  public class PaperMarioTheThousandYearDoorOptions
      : IMassExporterOptions<PaperMarioTheThousandYearDoorMassExporter> { }

  [Verb("pikmin_1", HelpText = "Extract models from Pikmin 1.")]
  public class Pikmin1Options
      : IMassExporterOptions<Pikmin1MassExporter> { }

  [Verb("pikmin_2", HelpText = "Extract models from Pikmin 2.")]
  public class Pikmin2Options : IMassExporterOptions<Pikmin2MassExporter> { }

[Verb("professor_layton_vs_phoenix_wright",
        HelpText = "Extract models from Professor Layton vs. Phoenix Wright.")]
  public class ProfessorLaytonVsPhoenixWrightOptions
      : IMassExporterOptions<ProfessorLaytonVsPhoenixWrightMassExporter> { }

  [Verb("super_mario_sunshine",
        HelpText = "Extract models from Super Mario Sunshine.")]
  public class SuperMarioSunshineOptions
      : IMassExporterOptions<SuperMarioSunshineMassExporter> { }

  [Verb("wind_waker",
        HelpText = "Extract models from Wind Waker.")]
  public class WindWakerOptions : IMassExporterOptions<WindWakerMassExporter> { }
}