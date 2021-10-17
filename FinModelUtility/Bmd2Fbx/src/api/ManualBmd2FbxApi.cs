using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bmd.exporter;
using bmd.GCN;

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
        float frameRate = 30,
        bool useStatic = true) {
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

      List<(string, BMD)> pathsAndBmds;
      try {
        pathsAndBmds = bmdPaths.Select(
                                   bmdPath => (bmdPath,
                                               new BMD(
                                                   File.ReadAllBytes(bmdPath))))
                               .ToList();
      } catch {
        logger.LogError("Failed to load BMD!");
        throw;
      }

      List<(string, IBcx)> pathsAndBcxs;
      try {
        pathsAndBcxs = bcxPaths
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
            btiPaths.Select(btiPath => (btiPath,
                                        new BTI(
                                            File.ReadAllBytes(btiPath))))
                    .ToList();
      } catch {
        logger.LogError("Failed to load BTI!");
        throw;
      }

      outputDirectory.Create();

      foreach (var (bmdPath, bmd) in pathsAndBmds) {
        var bmdFile = new FileInfo(bmdPath);
        BmdDebugHelper.ExportFilesInBmd(outputDirectory,
                                        bmd,
                                        bmdFile.Name.Substring(0, bmdFile.Name.Length - ".bmd".Length),
                                        pathsAndBtis);
      }

      if (useStatic) {
        logger.LogInformation("Converting to a static mesh first.");

        foreach (var (bmdPath, bmd) in pathsAndBmds) {
          try {
            var model =
                new ModelConverter().Convert(bmd,
                                             pathsAndBcxs,
                                             pathsAndBtis,
                                             frameRate);

            var bmdFile = new FileInfo(bmdPath);
            new AssimpIndirectExporter().Export(
                new FinFile(Path.Join(outputDirectory.FullName, bmdFile.Name))
                    .CloneWithExtension(".fbx"),
                model);
          } catch(Exception e) {
            logger.LogError(e.ToString());
          }
        }
      } else {
        logger.LogInformation("Exporting directly.");
        foreach (var pathAndBmd in pathsAndBmds) {
          new GltfExporterOld().Export(outputDirectory,
                                       pathAndBmd,
                                       pathsAndBcxs,
                                       pathsAndBtis);
        }
      }
    }
  }
}