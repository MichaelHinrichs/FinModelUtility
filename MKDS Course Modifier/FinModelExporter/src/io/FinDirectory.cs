using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.util.asserts;

namespace fin.io {
  public class FinDirectory : IDirectory {
    private readonly DirectoryInfo impl_;


    public static IDirectory GetCwd()
      => new FinDirectory(Directory.GetCurrentDirectory());


    public FinDirectory(DirectoryInfo directoryInfo)
      => this.impl_ = directoryInfo;

    public FinDirectory(string fullName)
      => this.impl_ = new DirectoryInfo(fullName);


    public string Name => this.impl_.Name;
    public string FullName => this.impl_.FullName;

    public IDirectory? GetParent() => new FinDirectory(this.impl_.Parent);

    public bool Exists => this.impl_.Exists;

    public bool Create() {
      if (this.Exists) {
        return false;
      }

      this.impl_.Create();
      return true;
    }

    public IEnumerable<IDirectory> GetExistingSubdirs()
      => this.impl_.EnumerateDirectories()
             .Select(subdir => new FinDirectory(subdir));

    public IDirectory TryToGetSubdir(string relativePath, bool create = false)
      => this.GetSubdirImpl_(relativePath.Split('/', '\\'), create);

    private FinDirectory GetSubdirImpl_(
        IEnumerable<string> subdirs,
        bool create) {
      var current = this.impl_;

      foreach (var subdir in subdirs) {
        if (subdir == "..") {
          current = current.Parent;
          continue;
        }

        var matches = current.GetDirectories(subdir);

        if (!create || matches.Length == 1) {
          current = matches.Single();
        } else {
          current = current.CreateSubdirectory(subdir);
        }
      }

      return new FinDirectory(current);
    }


    public IEnumerable<IFile> GetExistingFiles()
      => this.impl_.EnumerateFiles().Select(file => new FinFile(file));

    public IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => this.impl_
             .GetFiles(searchPattern,
                       includeSubdirs
                           ? SearchOption.AllDirectories
                           : SearchOption.TopDirectoryOnly)
             .Select(file => new FinFile(file));

    public IFile TryToGetFile(string path) {
      // TODO: Handle subdirectories automatically.
      try {
        return new FinFile(this.impl_.GetFiles(path).Single());
      } catch (Exception e) {
        throw new Exception($"Expected to find {path}", e);
      }
    }
  }
}