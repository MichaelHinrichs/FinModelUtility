using System.IO;
using System.Linq;

using fin.util.asserts;

namespace fin.io {
  public static class Files {
    public static DirectoryInfo GetCwd()
      => new DirectoryInfo(Directory.GetCurrentDirectory());

    public static FileInfo[] GetFilesWithExtension(
        DirectoryInfo directory,
        string extension)
      => directory.GetFiles($"*.{extension}", SearchOption.AllDirectories);

    public static string[] GetPathsWithExtension(
        DirectoryInfo directory,
        string extension)
      => Files.GetFilesWithExtension(directory, extension)
              .Select(file => file.FullName)
              .ToArray();

    public static string GetPathWithExtension(
        DirectoryInfo directory,
        string extension) {
      var paths = Files.GetPathsWithExtension(directory, extension);

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


    public static FileInfo[] GetFilesWithExtension(string extension)
      => Files.GetFilesWithExtension(Files.GetCwd(), extension);

    public static string[] GetPathsWithExtension(string extension)
      => Files.GetPathsWithExtension(Files.GetCwd(), extension);

    public static string GetPathWithExtension(string extension)
      => Files.GetPathWithExtension(Files.GetCwd(), extension);
  }
}