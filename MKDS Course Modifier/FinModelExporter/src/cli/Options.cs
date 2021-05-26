using CommandLine;

namespace fin.cli {
  public abstract class BBasicOptions {
    [Option("verbose",
            HelpText = "Whether to print verbose log output.",
            Required = false)]
    public bool Verbose { get; set; }

    [Option("static",
            HelpText =
                "Whether to extract a model to a static mesh before conversion. Currently a work-in-progress.",
            Required = false)]
    public bool Static { get; set; }
  }

  public abstract class BConversionOptions : BBasicOptions {
    [Option("out",
            HelpText =
                "Path to an output .glb/.gltf that will be generated.",
            Required = true)]
    public string OutputPath { get; set; }
  }

  [Verb("automatic",
        HelpText =
            "Convert GCN model with automatically-determined input files.")]
  public class AutomaticOptions : BConversionOptions {}

  [Verb("manual",
        HelpText = "Convert GCN model with manually-specified input files.")]
  public class ManualOptions : BConversionOptions {
    [Option("bmd",
            HelpText = "Path to an input .bmd model.",
            Required = true)]
    public string BmdPath { get; set; }

    [Option("bcx", HelpText = "Path(s) to input .bca/.bck animations.")]
    public string[] BcxPaths { get; set; }

    [Option("bti", HelpText = "Path(s) to input .bti textures.")]
    public string[] BtiPaths { get; set; }
  }

  [Verb("debug",
        HelpText =
            "Convert GCN model with hardcoded input files. Not meant for general use.")]
  public class DebugOptions : BBasicOptions {}
}