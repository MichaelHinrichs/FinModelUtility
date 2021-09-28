using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.cli;
using fin.exporter.assimp;
using fin.exporter.gltf;
using fin.io;
using fin.log;
using fin.util.asserts;

using Microsoft.Extensions.Logging;

using mkds.exporter;

using MKDS_Course_Modifier.GCN;

namespace mkds.cli {
  public class Cli {
    public static int Main(string[] args) {
      Args.PopulateFromArgs(args);

      var logger = Logging.Create<Cli>();
      using var _ = logger.BeginScope("Entry");
      logger.LogInformation(string.Join(" ", args));

      using var _2 = logger.BeginScope("Main");
      logger.LogInformation("Attempting to parse:");
      logger.LogInformation(
          $"- {Args.BmdPaths.Count} model(s):\n" +
          string.Join('\n', Args.BmdPaths));
      logger.LogInformation(
          $"- {Args.BcxPaths.Count} animation(s):\n" +
          string.Join('\n', Args.BcxPaths));
      logger.LogInformation(
          $"- {Args.BtiPaths.Count} external texture(s):\n" +
          string.Join('\n', Args.BtiPaths));

      Asserts.True(
          !Args.Automatic || (Args.BmdPaths.Count == 1 || !Args.BcxPaths.Any()),
          "While automatically gathering files for a directory, found " +
          "multiple BMDs and animations. Not sure which animations go with " +
          "which BMDs, so aborting this operation.");

      var nonexistentBmds =
          Args.BmdPaths.Where(bmdPath => !File.Exists(bmdPath));
      var bmdsExist = !nonexistentBmds.Any();
      if (!bmdsExist) {
        throw new ArgumentException("Some bmds don't exist: " +
                                    string.Join(' ', nonexistentBmds));
      }

      var nonexistentBcxes =
          Args.BcxPaths.Where(bcxPath => !File.Exists(bcxPath));
      var bcxesExist = !nonexistentBcxes.Any();
      if (!bcxesExist) {
        throw new ArgumentException("Some bcxes don't exist: " +
                                    string.Join(' ', nonexistentBcxes));
      }

      var nonexistentBtis =
          Args.BtiPaths.Where(btiPath => !File.Exists(btiPath));
      var btisExist = !nonexistentBtis.Any();
      if (!btisExist) {
        throw new ArgumentException("Some btis don't exist: " +
                                    string.Join(' ', nonexistentBtis));
      }

      List<(string, BMD)> pathsAndBmds;
      try {
        pathsAndBmds = Args.BmdPaths.Select(
                               bmdPath => (bmdPath,
                                           new BMD(File.ReadAllBytes(bmdPath))))
                           .ToList();
      } catch {
        logger.LogError("Failed to load BMD!");
        throw;
      }

      List<(string, IBcx)> pathsAndBcxs;
      try {
        pathsAndBcxs = Args.BcxPaths
                           .Select(bcxPath => {
                             var extension =
                                 new FileInfo(bcxPath).Extension.ToLower();
                             IBcx bcx = extension switch {
                                 ".bca" =>
                                     new BCA(File.ReadAllBytes(bcxPath)),
                                 ".bck" =>
                                     new BCK(File.ReadAllBytes(bcxPath)),
                                 _ => throw new NotSupportedException(),
                             };
                             return (bcxPath, bcx);
                           })
                           .ToList();
      } catch {
        logger.LogError("Failed to load BCX!");
        throw;
      }

      List<(string, BTI)> pathsAndBtis;
      try {
        pathsAndBtis =
            Args.BtiPaths.Select(btiPath => (btiPath,
                                             new BTI(
                                                 File.ReadAllBytes(btiPath))))
                .ToList();
      } catch {
        logger.LogError("Failed to load BTI!");
        throw;
      }

      var outputDirectory = Args.OutputDirectory;
      outputDirectory.Create();

      foreach (var (_, bmd) in pathsAndBmds) {
        BmdDebugHelper.ExportFilesInBmd(bmd, pathsAndBtis);
      }

      if (Args.Static) {
        logger.LogInformation("Converting to a static mesh first.");

        foreach (var (bmdPath, bmd) in pathsAndBmds) {
          var model =
              new ModelConverter().Convert(bmd, pathsAndBcxs, pathsAndBtis);

          //new GltfExporter().Export(outputFile, model);
          var bmdFile = new FileInfo(bmdPath);
          new AssimpExporter().Export(
              new FinFile(Path.Join(Args.OutputDirectoryPath, bmdFile.Name))
                  .CloneWithExtension(".fbx"),
              model);
        }
      } else {
        logger.LogInformation("Exporting directly.");
        foreach (var pathAndBmd in pathsAndBmds) {
          new GltfExporterOld().Export(Args.OutputDirectoryPath,
                                       pathAndBmd,
                                       pathsAndBcxs,
                                       pathsAndBtis);
        }
      }

      return 0;
    }
  }
}