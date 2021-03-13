using System.IO;
using System.Linq;

using mkds.util.asserts;

namespace mkds.io {
  public static class Files {
    public static DirectoryInfo GetCwd()
      => new DirectoryInfo(Directory.GetCurrentDirectory());

    public static FileInfo[] GetFilesWithExtension(string extension)
      => Files.GetCwd().GetFiles($"*.{extension}", SearchOption.AllDirectories);

    public static string[] GetPathsWithExtension(string extension)
      => Files.GetFilesWithExtension(extension)
              .Select(file => file.FullName)
              .ToArray();

    public static string GetPathWithExtension(string extension) {
      var paths = Files.GetPathsWithExtension(extension);

      var errorMessage =
          $"Expected to find a single '.{extension}' file within '{Files.GetCwd().FullName}' but found {paths.Length}";
      if (paths.Length == 0) {
        errorMessage += ".";
      } else {
        errorMessage += ":\n";
        errorMessage = paths.Aggregate(errorMessage,
                                       (current, path)
                                           => current + (path + "\n"));
      }

      Asserts.True(paths.Length == 1, errorMessage);

      return paths[0];
    }
  }
}