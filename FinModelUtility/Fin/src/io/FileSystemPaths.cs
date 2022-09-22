using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace fin.io {
  public interface ISubdirPaths {
    string AbsoluteSubdirPath { get; }
    IReadOnlyList<string> AbsoluteFilePaths { get; }
    IReadOnlyList<ISubdirPaths> Subdirs { get; }
  }

  public interface IFileSystemPaths {
    ISubdirPaths ListAllPopulatedSubdirs(string rootSubdirPath);
  }

  public class FileSystemPaths : IFileSystemPaths {
    public ISubdirPaths ListAllPopulatedSubdirs(
        string rootSubdirPath) {
      var allFiles = Directory.GetFiles(
                                  rootSubdirPath, "*",
                                  new EnumerationOptions {
                                      RecurseSubdirectories =
                                          true
                                  })
                              .ToList();
      allFiles.Sort();

      rootSubdirPath = Path.GetFullPath(rootSubdirPath);
      var root = new SubdirPaths(rootSubdirPath);

      var subdirStack = new Stack<SubdirPaths>();
      subdirStack.Push(root);

      foreach (var file in allFiles) {
        var directoryPath = Path.GetDirectoryName(file);

        SubdirPaths currentSubdir = subdirStack.Peek();
        // If file is in current directory...
        if (directoryPath == currentSubdir.AbsoluteSubdirPath) {
          // No action needed
          ;
        }
        // If file is in a different directory...
        else {
          while (!file.StartsWith(currentSubdir.AbsoluteSubdirPath + '\\')) {
            subdirStack.Pop();
            currentSubdir = subdirStack.Peek();
          }

          // Push new directories
          var basePath = currentSubdir.AbsoluteSubdirPath;
          for (var i = basePath.Length + 1; i < file.Length; ++i) {
            if (file[i] == '\\') {
              var subdirPath = file.Substring(0, i);
              var subdir = new SubdirPaths(subdirPath);
              currentSubdir.SubdirsImpl.Add(subdir);
              subdirStack.Push(subdir);

              currentSubdir = subdir;
            }
          }
        }

        currentSubdir.AbsoluteFilePathsImpl.Add(file);
      }

      return root;
    }

    private class SubdirPaths : ISubdirPaths {
      public SubdirPaths(string absoluteSubdirPath) {
        this.AbsoluteSubdirPath = absoluteSubdirPath;
      }

      public string AbsoluteSubdirPath { get; }

      public IReadOnlyList<string> AbsoluteFilePaths => AbsoluteFilePathsImpl;
      public IReadOnlyList<ISubdirPaths> Subdirs => SubdirsImpl;

      public List<string> AbsoluteFilePathsImpl { get; } = new();
      public List<ISubdirPaths> SubdirsImpl { get; } = new();
    }
  }
}