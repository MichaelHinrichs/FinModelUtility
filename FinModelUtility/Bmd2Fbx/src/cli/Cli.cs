using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bmd.api;
using bmd.exporter;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.log;
using fin.util.asserts;

using bmd.GCN;

namespace bmd.cli {
  public class Cli {
    public static int Main(string[] args) {
      var argsInstance = new Args();
      argsInstance.PopulateFromArgs(args);

      var logger = Logging.Create<Cli>();
      logger.LogInformation(string.Join(" ", args));
      logger.LogInformation(" ");

      new ManualBmd2FbxApi().Process(argsInstance.OutputDirectory,
                                     argsInstance.BmdPaths,
                                     argsInstance.BcxPaths,
                                     argsInstance.BtiPaths,
                                     argsInstance.Automatic,
                                     argsInstance.FrameRate,
                                     argsInstance.Static);

      return 0;
    }
  }
}