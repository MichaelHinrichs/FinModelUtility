using fin.io;
using fin.util.asserts;

namespace uni.platforms {
  public static class DirectoryConstants {
    public static IDirectory BASE_DIRECTORY =
        Asserts.CastNonnull(Files.GetCwd().GetAncestry())
               .Where(ancestor => {
                 var subdirNames = ancestor.GetExistingSubdirs()
                                           .Select(directory => directory.Name);
                 return subdirNames.Contains("cli") &&
                        subdirNames.Contains("FinModelUtility");
               })
               .Single();

    public static IDirectory CLI_DIRECTORY =
        DirectoryConstants.BASE_DIRECTORY.GetSubdir("cli");

    public static IFile CONFIG_FILE { get; } =
      CLI_DIRECTORY.GetExistingFile("config.json");


    public static IDirectory ROMS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("roms");

    public static IDirectory TOOLS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("tools");

    public static IDirectory OUT_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.GetSubdir("out");
  }
}