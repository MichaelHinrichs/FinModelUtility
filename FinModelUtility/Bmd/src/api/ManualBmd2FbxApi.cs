using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bmd.exporter;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.log;
using fin.util.asserts;


namespace bmd.api {
  public class ManualBmd2FbxApi {
    public void Process(
        IDirectory outputDirectory,
        IList<string> bmdPaths,
        IList<string> bcxPaths,
        IList<string> btiPaths,
        bool wasSourcedAutomatically = false,
        float frameRate = 30) {
      var logger = Logging.Create<ManualBmd2FbxApi>();
      logger.LogInformation("Attempting to parse:");
      logger.LogInformation(
          $"- {bmdPaths.Count} model(s):\n" +
          string.Join('\n', bmdPaths.Select(bmdPath => "    " + bmdPath)));
      logger.LogInformation(
          $"- {bcxPaths.Count} animation(s):\n" +
          string.Join('\n', bcxPaths.Select(bcxPath => "    " + bcxPath)));
      logger.LogInformation(
          $"- {btiPaths.Count} external texture(s):\n" +
          string.Join('\n', btiPaths.Select(btiPath => "    " + btiPath)));
      logger.LogInformation(" ");

      Asserts.True(
          !wasSourcedAutomatically || (bmdPaths.Count == 1 || !bcxPaths.Any()),
          "While automatically gathering files for a directory, found " +
          "multiple BMDs and animations. Not sure which animations go with " +
          "which BMDs, so aborting this operation.");

      var nonexistentBmds =
          bmdPaths.Where(bmdPath => !File.Exists(bmdPath));
      var bmdsExist = !nonexistentBmds.Any();
      if (!bmdsExist) {
        throw new ArgumentException("Some bmds don't exist: " +
                                    string.Join(' ', nonexistentBmds));
      }

      var nonexistentBcxes =
          bcxPaths.Where(bcxPath => !File.Exists(bcxPath));
      var bcxesExist = !nonexistentBcxes.Any();
      if (!bcxesExist) {
        throw new ArgumentException("Some bcxes don't exist: " +
                                    string.Join(' ', nonexistentBcxes));
      }

      var nonexistentBtis =
          btiPaths.Where(btiPath => !File.Exists(btiPath));
      var btisExist = !nonexistentBtis.Any();
      if (!btisExist) {
        throw new ArgumentException("Some btis don't exist: " +
                                    string.Join(' ', nonexistentBtis));
      }

      outputDirectory.Create();

      /*foreach (var bmdPath in bmdPaths) {
        var bmdFile = new FileInfo(bmdPath);
        BmdDebugHelper.ExportFilesInBmd(outputDirectory,
                                        bmd,
                                        bmdFile.Name.Substring(
                                            0,
                                            bmdFile.Name.Length -
                                            ".bmd".Length),
                                        pathsAndBtis);
      }*/

      foreach (var bmdPath in bmdPaths) {
        var bmdFile = new FinFile(bmdPath);

        var model =
            new BmdModelLoader()
                .LoadModel(new BmdModelFileBundle {
                    BmdFile = bmdFile,
                    BcxFiles =
                        bcxPaths
                            .Select(bcxPath => new FinFile(bcxPath))
                            .ToArray(),
                    BtiFiles =
                        btiPaths
                            .Select(btiPath => new FinFile(btiPath))
                            .ToArray(),
                    FrameRate = frameRate
                });

        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName, bmdFile.Name))
                .CloneWithExtension(".fbx"),
            model);
      }
    }
  }
}