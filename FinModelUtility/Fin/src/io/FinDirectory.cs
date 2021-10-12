using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fin.io {
  public class FinDirectory : IDirectory {
    public static IDirectory GetCwd()
      => new FinDirectory(Directory.GetCurrentDirectory());


    public FinDirectory(DirectoryInfo directoryInfo)
      => this.Info = directoryInfo;

    public FinDirectory(string fullName)
      => this.Info = new DirectoryInfo(fullName);

    public DirectoryInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

    public bool Exists => Directory.Exists(this.FullName);

    public IDirectory? GetParent()
      => this.Info.Parent != null
             ? new FinDirectory(this.Info.Parent)
             : null;

    public IDirectory[] GetAncestry() {
      var parents = new LinkedList<IDirectory>();
      IDirectory? parent = null;
      do {
        parent = parent == null ? this.GetParent() : parent.GetParent();
        if (parent != null) {
          parents.AddLast(parent);
        }
      } while (parent != null);
      return parents.ToArray();
    }

    public bool Create() {
      if (this.Exists) {
        return false;
      }

      this.Info.Create();
      return true;
    }

    public IEnumerable<IDirectory> GetExistingSubdirs()
      => this.Info.EnumerateDirectories()
             .Select(subdir => new FinDirectory(subdir));

    public IDirectory TryToGetSubdir(string relativePath, bool create = false)
      => this.GetSubdirImpl_(relativePath.Split('/', '\\'), create);

    private FinDirectory GetSubdirImpl_(
        IEnumerable<string> subdirs,
        bool create) {
      var current = this.Info;

      foreach (var subdir in subdirs) {
        if (subdir == "") {
          continue;
        }

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
      => this.Info.EnumerateFiles().Select(file => new FinFile(file));

    public IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => this.Info
             .GetFiles(searchPattern,
                       includeSubdirs
                           ? SearchOption.AllDirectories
                           : SearchOption.TopDirectoryOnly)
             .Select(file => new FinFile(file));

    public IFile TryToGetFile(string path) {
      // TODO: Handle subdirectories automatically.
      try {
        return new FinFile(this.Info.GetFiles(path).Single());
      } catch (Exception e) {
        throw new Exception($"Expected to find {path}", e);
      }
    }

    public override string ToString() => this.FullName;


    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }
      if (other is not IDirectory otherDirectory) {
        return false;
      }
      return this.Equals(otherDirectory);
    }

    public bool Equals(IDirectory other) {
      return Path.GetFullPath(this.FullName) ==
             Path.GetFullPath(other.FullName);
    }

    public override int GetHashCode() => this.FullName.GetHashCode();

    public static bool operator ==(FinDirectory lhs, IDirectory rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinDirectory lhs, IDirectory rhs)
      => !lhs.Equals(rhs);
  }
}