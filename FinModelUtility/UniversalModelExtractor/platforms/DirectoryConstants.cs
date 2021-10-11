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

    public static IDirectory TOOLS_DIRECTORY =
        DirectoryConstants.BASE_DIRECTORY.TryToGetSubdir("cli/tools");
  }
}