using bmd.api;

using fin.log;

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
                                     argsInstance.FrameRate);

      return 0;
    }
  }
}