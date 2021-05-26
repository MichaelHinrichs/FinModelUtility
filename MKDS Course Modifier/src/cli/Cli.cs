using System;
using System.IO;
using System.Linq;

using fin.cli;
using fin.exporter.gltf;
using fin.log;

using Microsoft.Extensions.Logging;

using mkds.exporter;

using MKDS_Course_Modifier.GCN;

namespace mkds.cli {
  public class Cli {
    public static int Main(string[] args) {
      try {
        Args.PopulateFromArgs(args);
      } catch {
        return 1;
      }

      var logger = Logging.Create<Cli>();
      using var _ = logger.BeginScope("Main");

      logger.LogInformation(string.Join(" ", args));

      var bmd = new BMD(File.ReadAllBytes(Args.BmdPath));
      var pathsAndBcxs = Args.BcxPaths
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
      var pathsAndBtis =
          Args.BtiPaths.Select(btiPath => (btiPath,
                                           new BTI(File.ReadAllBytes(btiPath))))
              .ToList();

      var useStaticModel = true;
      if (useStaticModel) {
        var model =
            new ModelConverter().Convert(bmd, pathsAndBcxs, pathsAndBtis);
        new GltfExporter().Export(Args.OutputPath, model);
      } else {
        new GltfExporterOld().Export(Args.OutputPath,
                                     bmd,
                                     pathsAndBcxs,
                                     pathsAndBtis);
      }

      return 0;
    }
  }
}