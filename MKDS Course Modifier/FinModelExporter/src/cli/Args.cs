using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;
using CommandLine.Text;

using fin.io;
using fin.log;
using fin.util.array;

namespace fin.cli {
  // TODO: Hook downstream classes into this for args by system.
  public static class Args {
    public static bool Verbose { get; private set; }
    public static string OutputPath { get; private set; } = "";
    public static string BmdPath { get; private set; } = "";

    public static IList<string> BcxPaths { get; private set; } =
      new List<string>();

    public static IList<string> BtiPaths { get; private set; } =
      new List<string>();

    /// <summary>
    ///   Populates the static args from the command line arguments passed in.
    ///
    ///   Throws an error if parsing failed.
    /// </summary>
    public static void PopulateFromArgs(string[] args) {
      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AutomaticOptions),
                    typeof(ManualOptions),
                    typeof(DebugOptions))
                .WithParsed((AutomaticOptions automaticOpts) => {
                  Args.Verbose = automaticOpts.Verbose;
                  Args.OutputPath = automaticOpts.OutputPath;
                  Args.BmdPath = Files.GetPathWithExtension("bmd");
                  Args.BcxPaths = Arrays.Concat(Files.GetPathsWithExtension(
                                                    "bca"),
                                                Files.GetPathsWithExtension(
                                                    "bck"));
                  Args.BtiPaths = Files.GetPathsWithExtension("bti");
                })
                .WithParsed((ManualOptions manualOpts) => {
                  Args.Verbose = manualOpts.Verbose;
                  Args.OutputPath = manualOpts.OutputPath;
                  Args.BmdPath = manualOpts.BmdPath;
                  Args.BcxPaths = manualOpts.BcxPaths;
                  Args.BtiPaths = manualOpts.BtiPaths;
                })
                .WithParsed((DebugOptions debugOpts) => {
                  Args.Verbose = debugOpts.Verbose;
                  Args.GetForEnemy_("Chappy",
                                    out var outputPath,
                                    out var bmdPath,
                                    out var bcxPaths,
                                    out var btiPaths);
                  /*Args.GetForEnemy("Queen",
                                   out var outputPath,
                                   out var bmdPath,
                                   out var bcxPaths,
                                   out var btiPaths);
                  /*Args.GetFromDirectory(
                      new DirectoryInfo(
                          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/roms/pkmn2.gcm_dir/user/Kando/ufo"),
                      out var outputPath,
                      out var bmdPath,
                      out var bcxPaths,
                      out var btiPaths);*/
                  //GetForPikmin(out outputPath, out bmdPath);
                  //GetForTesting(out outputPath, out bmdPath, out bcxPaths);

                  Args.OutputPath = outputPath;
                  Args.BmdPath = bmdPath;
                  Args.BcxPaths = bcxPaths;
                  Args.BtiPaths = btiPaths;
                })
                .WithNotParsed(parseErrors => errors = parseErrors);

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        throw new Exception();
      }

      Logging.Initialize(Args.Verbose);
    }

    private static void GetForTesting_(
        out string outputPath,
        out string bmdPath,
        out IList<string> bcxPaths) {
      outputPath =
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/out.glb";
      bmdPath =
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/enemy.bmd";
      bcxPaths = new[] {
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack0.bca",
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack1.bca",
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack4.bca",
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/flick.bca",
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/move.bca"
      };
    }

    private static void GetForEnemy_(
        string name,
        out string outputPath,
        out string bmdPath,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      outputPath = Args.GetOutputPath_(name);

      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      var enemyBasePath =
          $"{basePath}cli/roms/pkmn2.gcm_dir/enemy/data/{name}/";

      Args.GetFromDirectory_(new DirectoryInfo(enemyBasePath),
                            out outputPath,
                            out bmdPath,
                            out bcxPaths,
                            out btiPaths);
    }

    private static void GetForPikmin_(
        out string outputPath,
        out string bmdPath) {
      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      outputPath = $"{basePath}cli/out/Pikmin/Red/red.glb";

      var pikminBasePath =
          $"{basePath}cli/roms/pkmn2.gcm_dir/user/Kando/piki/pikis.szs 0.rarc_dir/designer/piki_model/";
      bmdPath = $"{pikminBasePath}piki_p2_red.bmd";

      /*var bcxFiles =
          new DirectoryInfo($"{enemyBasePath}anim.szs 0.rarc_dir/anim/");
      bcxPaths = bcxFiles.GetFiles().Select(file => file.FullName).ToList();
  
      btiPaths = new DirectoryInfo(enemyBasePath)
                 .GetFiles()
                 .SkipWhile(file => file.Extension != ".bti")
                 .Select(file => file.FullName)
                 .ToList();*/
    }

    private static string GetOutputPath_(string name) {
      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      return $"{basePath}cli/out/{name}/{name}.glb";
    }

    private static void GetFromDirectory_(
        DirectoryInfo directory,
        out string outputPath,
        out string bmdPath,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      outputPath = Args.GetOutputPath_(directory.Name);

      bmdPath = Files.GetPathWithExtension(directory, "bmd");
      bcxPaths = Arrays.Concat(
          Files.GetPathsWithExtension(directory, "bca"),
          Files.GetPathsWithExtension(directory, "bck"));
      btiPaths = Files.GetPathsWithExtension(directory, "bti");
    }
  }
}