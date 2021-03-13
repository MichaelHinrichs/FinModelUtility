using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using CommandLine;
using CommandLine.Core;
using CommandLine.Infrastructure;
using CommandLine.Text;

using mkds.exporter;
using mkds.io;
using mkds.util.array;

using MKDS_Course_Modifier.GCN;

namespace mkds.cli {
  public class Cli {
    public static int Main(string[] args) {
      string outputPath = "";
      string bmdPath = "";
      IList<string> bcxPaths = new List<string>();
      IList<string> btiPaths = new List<string>();

      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AutomaticOptions),
                    typeof(ManualOptions))
                .WithParsed((AutomaticOptions automaticOpts)
                                => {
                              outputPath = automaticOpts
                                  .OutputPath;
                              bmdPath =
                                  Files.GetPathWithExtension(
                                      "bmd");
                              bcxPaths = Arrays.Concat(
                                  Files.GetPathsWithExtension(
                                      "bca"),
                                  Files.GetPathsWithExtension(
                                      "bck"));
                              btiPaths =
                                  Files.GetPathsWithExtension(
                                      "bti");
                            })
                .WithParsed((ManualOptions manualOpts) => {
                  outputPath = manualOpts.OutputPath;
                  bmdPath = manualOpts.BmdPath;
                  bcxPaths = manualOpts.BcxPaths;
                  btiPaths = manualOpts.BtiPaths;
                })
                .WithNotParsed((IEnumerable<Error> parseErrors)
                                   => {
                                 // If debugger is not attached, assume we are
                                 // running from the command line.
                                 if (!Debugger.IsAttached) {
                                   errors = parseErrors;
                                   return;
                                 }

                                 Cli.GetForEnemy("Queen",
                                                 out outputPath,
                                                 out bmdPath,
                                                 out bcxPaths,
                                                 out btiPaths);
                                 //GetForPikmin(out outputPath, out bmdPath);
                                 //GetForTesting(out outputPath, out bmdPath, out bcxPaths);
                               });

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        return 1;
      }

      var bmd = new BMD(File.ReadAllBytes(bmdPath));
      var pathsAndBcxs = bcxPaths
                         .Select(bcxPath => {
                           var extension =
                               new FileInfo(bcxPath).Extension.ToLower();
                           IBcx bcx = extension switch {
                               ".bca" => new BCA(File.ReadAllBytes(bcxPath)),
                               ".bck" => new BCK(File.ReadAllBytes(bcxPath)),
                               _      => throw new NotSupportedException(),
                           };
                           return (bcxPath, bcx);
                         })
                         .ToList();
      var pathsAndBtis =
          btiPaths.Select(btiPath => (btiPath,
                                      new BTI(File.ReadAllBytes(btiPath))))
                  .ToList();

      GltfExporter.Export(outputPath, bmd, pathsAndBcxs, pathsAndBtis);

      return 0;
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