using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommandLine;
using CommandLine.Text;

using fin.io;
using fin.log;
using fin.util.array;

namespace bmd.cli {
  // TODO: Hook downstream classes into this for args by system.
  public class Args {
    public bool Automatic { get; private set; }
    public bool Static { get; private set; }
    public float FrameRate { get; private set; } = 30;
    public bool Verbose { get; private set; }

    public string OutputDirectoryPath { get; private set; } = "";

    public IDirectory OutputDirectory
      => new FinDirectory(this.OutputDirectoryPath);

    public IList<string> BmdPaths { get; private set; } =
      new List<string>();

    public IList<string> BcxPaths { get; private set; } =
      new List<string>();

    public IList<string> BtiPaths { get; private set; } =
      new List<string>();

    /// <summary>
    ///   Populates the static args from the command line arguments passed in.
    ///
    ///   Throws an error if parsing failed.
    /// </summary>
    public void PopulateFromArgs(string[] args) {
      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AutomaticOptions),
                    typeof(ManualOptions),
                    typeof(DebugOptions))
                .WithParsed((AutomaticOptions automaticOpts) => {
                  this.Automatic = true;
                  this.Static = automaticOpts.Static;
                  this.FrameRate = automaticOpts.FrameRate ?? 30;
                  this.Verbose = automaticOpts.Verbose;
                  this.OutputDirectoryPath = automaticOpts.OutputPath;
                  this.BcxPaths = Arrays.Concat(Files.GetPathsWithExtension(
                                                    "bca",
                                                    true),
                                                Files.GetPathsWithExtension(
                                                    "bck",
                                                    true));
                  this.BtiPaths = Files.GetPathsWithExtension("bti", true);
                  this.BmdPaths = Files.GetPathsWithExtension("bmd", true);
                })
                .WithParsed((ManualOptions manualOpts) => {
                  this.Automatic = false;
                  this.Static = manualOpts.Static;
                  this.FrameRate = manualOpts.FrameRate ?? 30;
                  this.Verbose = manualOpts.Verbose;
                  this.OutputDirectoryPath = manualOpts.OutputPath;
                  this.BmdPaths = manualOpts.BmdPaths.ToList();
                  this.BcxPaths = manualOpts.BcxPaths.ToList();
                  this.BtiPaths = manualOpts.BtiPaths.ToList();
                })
                .WithParsed((DebugOptions debugOpts) => {
                  this.Automatic = false;
                  this.Static = debugOpts.Static;
                  this.Verbose = debugOpts.Verbose;


                  Args.GetForEnemy_("Chappy",
                                    out var outputPath,
                                    out var bmdPaths,
                                    out var bcxPaths,
                                    out var btiPaths);


                  /*this.FrameRate = 60;
                  var outputPath =
                      @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\super_mario_sunshine";
                  var bmdPaths = new String[] {
                      @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\super_mario_sunshine.gcm_dir\data\scene\bianco1.szs 0.rarc_dir\scene\map\map\map.bmd"
                      //@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\super_mario_sunshine.gcm_dir\data\mario.szs 0.rarc_dir\mario\bmd\ma_mdl1.bmd",
                  };
                  var bcxPaths = new String[] {};
                  var btiPaths = new String[] {};*/


                  /*var bcxPaths =
                      Files.GetFilesWithExtension(
                               new FinDirectory(
                                   @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\super_mario_sunshine.gcm_dir\data\mario.szs 0.rarc_dir\mario\bck"),
                               "bck")
                           .Where(
                               file => file.Name.StartsWith("ma_"))
                           .Select(file => file.FullName)
                           .ToArray();*/

                  /*Args.GetFromKando_("Ufo",
                                  out var outputPath,
                                  out var bmdPath,
                                  out var bcxPaths,
                                  out var btiPaths);*/

                  /*Args.GetForEnemy_("Queen", //"Queen"
                                    out var outputPath,
                                    out var bmdPaths,
                                    out var bcxPaths,
                                    out var btiPaths);

                  /*Args.GetFromDirectory_(
                      new DirectoryInfo(
                          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn2.gcm_dir\user\Kando\objects\bridge"),
                      out var outputPath,
                      out var bmdPaths,
                      out var bcxPaths,
                      out var btiPaths);*/

                  /*Args.GetFromDirectory_(
                      new DirectoryInfo(
                          "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/roms/pkmn2.gcm_dir/user/Kando/piki/pikis.szs 0.rarc_dir/designer/happa_model"),
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

                  this.OutputDirectoryPath = outputPath;
                  this.BmdPaths = bmdPaths;
                  this.BcxPaths = bcxPaths;
                  this.BtiPaths = btiPaths;
                })
                .WithNotParsed(parseErrors => errors = parseErrors);

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        throw new Exception();
      }

      Logging.Initialize(this.Verbose);
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
          $"{basePath}cli/roms/pikmin_2.gcm_dir/enemy/data/{name}/";

      Args.GetFromDirectory_(new FinDirectory(enemyBasePath),
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
      return $"{basePath}cli/out/pkmn2/{name}/";
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

      bmdPaths = Files.GetPathsWithExtension(modelDir, "bmd", true);
      bcxPaths = Arrays.Concat(
          Files.GetPathsWithExtension(modelDir, "bca", true),
          Files.GetPathsWithExtension(modelDir, "bck", true));
      btiPaths = Files.GetPathsWithExtension(modelDir, "bti", true);
    }

    private static void GetFromDirectory_(
        IDirectory directory,
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