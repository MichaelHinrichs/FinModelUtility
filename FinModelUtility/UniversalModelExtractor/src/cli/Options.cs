using CommandLine;

namespace uni.cli {
  [Verb("ocarina_of_time_3d",
        HelpText = "Extract models from Ocarina of Time 3d.")]
  public class OcarinaOfTime3dOptions { }

  [Verb("pikmin_1", HelpText = "Extract models from Pikmin 1.")]
  public class Pikmin1Options {}

  [Verb("pikmin_2", HelpText = "Extract models from Pikmin 2.")]
  public class Pikmin2Options {}

  [Verb("super_mario_sunshine",
        HelpText = "Extract models from Super Mario Sunshine.")]
  public class SuperMarioSunshineOptions {}

  [Verb("debug",
        HelpText =
            "Extract models with hardcoded input files. Not meant for general use.")]
  public class DebugOptions {}
}