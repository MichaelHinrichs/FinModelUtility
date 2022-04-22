using CommandLine;

namespace uni.cli {
  [Verb("animal_crossing",
        HelpText = "Extract models from Animal Crossing.")]
  public class AnimalCrossingOptions {}

  [Verb("battalion_wars",
        HelpText = "Extract models from Battalion Wars.")]
  public class BattalionWarsOptions {}

  [Verb("halo_wars",
        HelpText = "Extract models from Halo Wars.")]
  public class HaloWarsOptions {}

  [Verb("glover",
      HelpText = "Extract models from Glover.")]
  public class GloverOptions { }

  [Verb("luigis_mansion", HelpText = "Extract models from Luigi's Mansion.")]
  public class LuigisMansionOptions {}

  [Verb("luigis_mansion_3d",
        HelpText = "Extract models from Luigi's Mansion 3D.")]
  public class LuigisMansion3dOptions {}

  [Verb("mario_kart_double_dash",
        HelpText = "Extract models from Mario Kart: Double Dash.")]
  public class MarioKartDoubleDashOptions {}

  [Verb("ocarina_of_time_3d",
        HelpText = "Extract models from Ocarina of Time 3D.")]
  public class OcarinaOfTime3dOptions {}

  [Verb("ocarina_of_time", HelpText = "Extract models from Ocarina of Time.")]
  public class OcarinaOfTimeOptions {}

  [Verb("paper_mario_the_thousand_year_door",
        HelpText = "Extract models from Paper Mario: The Thousand Year Door.")]
  public class PaperMarioTheThousandYearDoorOptions {}

  [Verb("pikmin_1", HelpText = "Extract models from Pikmin 1.")]
  public class Pikmin1Options {}

  [Verb("pikmin_2", HelpText = "Extract models from Pikmin 2.")]
  public class Pikmin2Options {}

  [Verb("super_mario_sunshine",
        HelpText = "Extract models from Super Mario Sunshine.")]
  public class SuperMarioSunshineOptions {}

  [Verb("wind_waker",
        HelpText = "Extract models from Wind Waker.")]
  public class WindWakerOptions {}

  [Verb("debug",
        HelpText =
            "Extract models with hardcoded input files. Not meant for general use.")]
  public class DebugOptions {}
}