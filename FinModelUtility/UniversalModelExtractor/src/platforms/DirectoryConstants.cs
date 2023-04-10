using fin.io;
using fin.util.asserts;

namespace uni.platforms {
  public static class DirectoryConstants {
    public static ISystemDirectory BASE_DIRECTORY { get; } =
      DirectoryConstants.GetBaseDirectory_();

    private static ISystemDirectory GetBaseDirectory_() {
      var cwd = Files.GetCwd();
      if (cwd.Name == "FinModelUtility") {
        return cwd;
      }

      if (cwd.Name == "cli") {
        return cwd.GetParent();
      }

      if (cwd.Name == "universal_model_extractor") {
        return cwd.GetParent() // tools
                  .GetParent() // cli
                  .GetParent(); // FinModelUtility
      }

      return
          Asserts
              .CastNonnull(Files.GetCwd().GetAncestry())
              .Where(ancestor => {
                var subdirNames = ancestor
                                  .GetExistingSubdirs()
                                  .Select(
                                      directory
                                          => directory.Name);
                return subdirNames.Contains("cli") &&
                       subdirNames.Contains("FinModelUtility");
              })
              .Single();
    }

    public static ISystemDirectory CLI_DIRECTORY =
        DirectoryConstants.BASE_DIRECTORY.GetSubdir("cli");


    public static ISystemDirectory GAME_CONFIG_DIRECTORY { get; } =
      CLI_DIRECTORY.GetSubdir("config");

    public static ISystemFile CONFIG_FILE { get; } =
      DirectoryConstants.CLI_DIRECTORY.GetExistingFile("config.json");


    public static ISystemDirectory ROMS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("roms");

    public static ISystemDirectory TOOLS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("tools");

    public static ISystemDirectory OUT_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("out");
  }
}