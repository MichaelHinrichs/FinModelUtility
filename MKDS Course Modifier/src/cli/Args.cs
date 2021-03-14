using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using CommandLine;
using CommandLine.Text;

using mkds.io;
using mkds.util.array;

namespace mkds.cli {
  public static class Args {
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
                    typeof(ManualOptions))
                .WithParsed((AutomaticOptions automaticOpts) => {
                  Args.OutputPath = automaticOpts.OutputPath;
                  Args.BmdPath = Files.GetPathWithExtension("bmd");
                  Args.BcxPaths = Arrays.Concat(Files.GetPathsWithExtension(
                                                    "bca"),
                                                Files.GetPathsWithExtension(
                                                    "bck"));
                  Args.BtiPaths = Files.GetPathsWithExtension("bti");
                })
                .WithParsed((ManualOptions manualOpts) => {
                  Args.OutputPath = manualOpts.OutputPath;
                  Args.BmdPath = manualOpts.BmdPath;
                  Args.BcxPaths = manualOpts.BcxPaths;
                  Args.BtiPaths = manualOpts.BtiPaths;
                })
                .WithNotParsed((IEnumerable<Error> parseErrors) => {
                  // If debugger is not attached, assume we are running from
                  // the command line.
                  if (!Debugger.IsAttached) {
                    errors = parseErrors;
                    return;
                  }

                  Args.GetForEnemy("Pelplant",
                                   out var outputPath,
                                   out var bmdPath,
                                   out var bcxPaths,
                                   out var btiPaths);
                  //GetForPikmin(out outputPath, out bmdPath);
                  //GetForTesting(out outputPath, out bmdPath, out bcxPaths);

                  Args.OutputPath = outputPath;
                  Args.BmdPath = bmdPath;
                  Args.BcxPaths = bcxPaths;
                  Args.BtiPaths = btiPaths;
                });

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        throw new Exception();
      }
    }

    private static void GetForTesting(
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

    private static void GetForEnemy(
        string name,
        out string outputPath,
        out string bmdPath,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      outputPath = $"{basePath}cli/out/{name}/{name}.glb";

      var enemyBasePath =
          $"{basePath}cli/roms/pkmn2.gcm_dir/enemy/data/{name}/";
      bmdPath = $"{enemyBasePath}model.szs 0.rarc_dir/model/enemy.bmd";

      var bcxFiles =
          new DirectoryInfo($"{enemyBasePath}anim.szs 0.rarc_dir/anim/");
      bcxPaths = bcxFiles.GetFiles().Select(file => file.FullName).ToList();

      btiPaths = new DirectoryInfo(enemyBasePath)
                 .GetFiles()
                 .SkipWhile(file => file.Extension != ".bti")
                 .Select(file => file.FullName)
                 .ToList();
    }

    private static void GetForPikmin(
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
  }
}