using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommandLine;
using CommandLine.Text;

using fin.io;
using fin.log;
using fin.util.array;

namespace fin.cli {
  // TODO: Hook downstream classes into this for args by system.
  public static class Args {
    public static bool Automatic { get; private set; }
    public static bool Static { get; private set; }
    public static bool Verbose { get; private set; }

    public static string OutputDirectoryPath { get; private set; } = "";

    public static IDirectory OutputDirectory
      => new FinDirectory(Args.OutputDirectoryPath);

    public static IList<string> BmdPaths { get; private set; } =
      new List<string>();

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
                  Args.Automatic = true;
                  Args.Static = automaticOpts.Static;
                  Args.Verbose = automaticOpts.Verbose;
                  Args.OutputDirectoryPath = automaticOpts.OutputPath;
                  Args.BcxPaths = Arrays.Concat(Files.GetPathsWithExtension(
                                                    "bca",
                                                    true),
                                                Files.GetPathsWithExtension(
                                                    "bck",
                                                    true));
                  Args.BtiPaths = Files.GetPathsWithExtension("bti", true);
                  Args.BmdPaths = Files.GetPathsWithExtension("bmd", true);
                })
                .WithParsed((ManualOptions manualOpts) => {
                  Args.Automatic = false;
                  Args.Static = manualOpts.Static;
                  Args.Verbose = manualOpts.Verbose;
                  Args.OutputDirectoryPath = manualOpts.OutputPath;
                  Args.BmdPaths = manualOpts.BmdPaths.ToList();
                  Args.BcxPaths = manualOpts.BcxPaths.ToList();
                  Args.BtiPaths = manualOpts.BtiPaths.ToList();
                })
                .WithParsed((DebugOptions debugOpts) => {
                  Args.Automatic = false;
                  Args.Static = debugOpts.Static;
                  Args.Verbose = debugOpts.Verbose;

                  /*Args.GetForEnemy_("Chappy",
                                    out var outputPath,
                                    out var bmdPath,
                                    out var bcxPaths,
                                    out var btiPaths);

                  /*Args.GetFromKando_("Ufo",
                                  out var outputPath,
                                  out var bmdPath,
                                  out var bcxPaths,
                                  out var btiPaths);*/

                  /*Args.GetForEnemy_("Queen",
                                    out var outputPath,
                                    out var bmdPaths,
                                    out var bcxPaths,
                                    out var btiPaths);*/

                  Args.GetFromDirectory_(
                      new DirectoryInfo(
                          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn2.gcm_dir\user\Kando\objects\bridge"),
                      out var outputPath,
                      out var bmdPaths,
                      out var bcxPaths,
                      out var btiPaths);

                  /*Args.GetFromDirectory_(
                      new DirectoryInfo(
                          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/roms/pkmn2.gcm_dir/user/Kando/map/forest"),
                      out var outputPath,
                      out var bmdPaths,
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

                  Args.OutputDirectoryPath = outputPath;
                  Args.BmdPaths = bmdPaths;
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
        out IList<string> bmdPaths,
        out IList<string> bcxPaths) {
      outputPath =
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/out.glb";
      bmdPaths = new[] {
          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/enemy.bmd"
      };
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
        out IList<string> bmdPaths,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      outputPath = Args.GetOutputPath_(name);

      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      var enemyBasePath =
          $"{basePath}cli/roms/pkmn2.gcm_dir/enemy/data/{name}/";

      Args.GetFromDirectory_(new DirectoryInfo(enemyBasePath),
                             out outputPath,
                             out bmdPaths,
                             out bcxPaths,
                             out btiPaths);
    }

    private static void GetForPikmin_(
        out string outputPath,
        out IList<string> bmdPaths) {
      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      outputPath = $"{basePath}cli/out/Pikmin/Red/red.glb";

      var pikminBasePath =
          $"{basePath}cli/roms/pkmn2.gcm_dir/user/Kando/piki/pikis.szs 0.rarc_dir/designer/piki_model/";
      bmdPaths = new[] {$"{pikminBasePath}piki_p2_red.bmd"};

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
      return $"{basePath}cli/out/{name}/";
    }

    private static void GetFromKando_(
        string name,
        out string outputPath,
        out IList<string> bmdPaths,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      outputPath = Args.GetOutputPath_(name);

      var kandoDir = new FinDirectory(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn2.gcm_dir\user\Kando");
      var entryDir = kandoDir.TryToGetSubdir(name);
      var modelDir = entryDir.TryToGetSubdir(@"arc.szs 0.rarc_dir\arc");

      var modelDirFullName = modelDir.FullName;
      var modelDirInfo = new DirectoryInfo(entryDir.FullName);

      bmdPaths = Files.GetPathsWithExtension(modelDirInfo, "bmd", true);
      bcxPaths = Arrays.Concat(
          Files.GetPathsWithExtension(modelDirInfo, "bca", true),
          Files.GetPathsWithExtension(modelDirInfo, "bck", true));
      btiPaths = Files.GetPathsWithExtension(modelDirInfo, "bti", true);
    }

    private static void GetFromDirectory_(
        DirectoryInfo directory,
        out string outputPath,
        out IList<string> bmdPaths,
        out IList<string> bcxPaths,
        out IList<string> btiPaths) {
      outputPath = Args.GetOutputPath_(directory.Name);

      bmdPaths = Files.GetPathsWithExtension(directory, "bmd", true);
      bcxPaths = Arrays.Concat(
          Files.GetPathsWithExtension(directory, "bca", true),
          Files.GetPathsWithExtension(directory, "bck", true));
      btiPaths = Files.GetPathsWithExtension(directory, "bti", true);
    }
  }
}