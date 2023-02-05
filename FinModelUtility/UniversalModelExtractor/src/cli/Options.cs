using CommandLine;

namespace uni.cli {
  [Verb("ui", HelpText = "Open UI")]
  public class UiOptions { }

  [Verb("debug",
        HelpText =
            "Extract models with hardcoded input files. Not meant for general use.")]
  public class DebugOptions {}
}