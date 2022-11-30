using fin.log;

using mod.api;

namespace mod.cli {
  public class Cli {
    public static int Main(string[] args) {
      var argsInstance = new Args();
      argsInstance.PopulateFromArgs(args);

      var logger = Logging.Create<Cli>();
      logger.LogInformation(string.Join(" ", args));
      logger.LogInformation(" ");

      new ManualMod2FbxApi().Process(argsInstance.OutputDirectory,
                                     argsInstance.ModFiles,
                                     argsInstance.AnmFiles,
                                     argsInstance.Automatic);

      return 0;
    }
  }
}