using System;
using System.Linq;

using fin.util.asserts;

namespace fin.io {
  public static class Files {
    public static ISystemDirectory GetCwd()
      => new FinDirectory(FinFileSystem.Directory.GetCurrentDirectory());

    private static readonly object RUN_IN_DIRECTORY_LOCK_ = new();

    public static void RunInDirectory(ISystemDirectory directory, Action handler) {
      lock (RUN_IN_DIRECTORY_LOCK_) {
        var cwd = FinFileSystem.Directory.GetCurrentDirectory();

        Asserts.True(directory.Exists,
                     $"Attempted to run in nonexistent directory: {directory}");
        FinFileSystem.Directory.SetCurrentDirectory(directory.FullName);

        try {
          handler();
        } catch {
          FinFileSystem.Directory.SetCurrentDirectory(cwd);
          throw;
        }

        FinFileSystem.Directory.SetCurrentDirectory(cwd);
      }
    }


    // Getting Files
    public static ISystemFile[] GetFilesWithExtension(
        string extension,
        bool includeSubdirs = false)
      => Files.GetFilesWithExtension(Files.GetCwd(), extension, includeSubdirs);

    public static string AssertValidExtension(string extension) {
      Asserts.True(extension.StartsWith("."));
      return extension;
    }

    public static ISystemFile[] GetFilesWithExtension(
        ISystemDirectory directory,
        string extension,
        bool includeSubdirs = false)
      => directory.GetFilesWithExtension(extension, includeSubdirs)
                  .ToArray();

    public static ISystemFile GetFileWithExtension(
        ISystemDirectory directory,
        string extension,
        bool includeSubdirs = false)
      => new FinFile(
          Files.GetPathWithExtension(directory, extension, includeSubdirs));


    public static string[] GetPathsWithExtension(
        ISystemDirectory directory,
        string extension,
        bool includeSubdirs = false)
      => Files.GetFilesWithExtension(directory, extension, includeSubdirs)
              .Select(file => file.FullName)
              .ToArray();

    public static string GetPathWithExtension(
        ISystemDirectory directory,
        string extension,
        bool includeSubdirs = false) {
      var paths =
          Files.GetPathsWithExtension(directory, extension, includeSubdirs);

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

    public static string[] GetPathsWithExtension(
        string extension,
        bool includeSubdirs = false)
      => Files.GetPathsWithExtension(Files.GetCwd(), extension, includeSubdirs);

    public static string GetPathWithExtension(
        string extension,
        bool includeSubdirs = false)
      => Files.GetPathWithExtension(Files.GetCwd(), extension, includeSubdirs);
  }
}