using System.Linq;

using fin.io;

namespace uni.platforms {
  public static class DirectoryConstants {
    public static IDirectory BASE_DIRECTORY =
        Files.GetCwd()
             .GetAncestry()
             .Where(ancestor => {
               var subdirNames = ancestor.GetExistingSubdirs()
                                         .Select(directory => directory.Name);
               return subdirNames.Contains("cli") &&
                      subdirNames.Contains("FinModelUtility");
             })
             .Single();

    public static IDirectory CLI_DIRECTORY =
        DirectoryConstants.BASE_DIRECTORY.TryToGetSubdir("cli");

    public static IDirectory ROMS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.TryToGetSubdir("roms");

    public static IDirectory TOOLS_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.TryToGetSubdir("tools");

    public static IDirectory OUT_DIRECTORY =
        DirectoryConstants.CLI_DIRECTORY.TryToGetSubdir("out");
  }
}