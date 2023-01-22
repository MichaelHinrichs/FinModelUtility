using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;


namespace fin.io {
  public class FinDirectory : IDirectory {
    public static IDirectory GetCwd()
      => new FinDirectory(FinFileSystem.Directory.GetCurrentDirectory());


    public FinDirectory(IDirectoryInfo directoryInfo)
      => this.Info = directoryInfo;

    public FinDirectory(string fullName)
      => this.Info = FinFileSystem.Directory.CreateDirectory(fullName);

    public IDirectoryInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

    private string? absolutePath_ = null;

    public string GetAbsolutePath() {
      if (this.absolutePath_ == null) {
        this.absolutePath_ = Path.GetFullPath(this.FullName);
      }

      return this.absolutePath_;
    }

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

    public void MoveTo(string path) {
      try {
        this.Info.MoveTo(path);
      }
      // Sometimes the first move throws a permission denied error, so we just need to try again.
      catch {
        this.Info.MoveTo(path);
      }
    }

    public IEnumerable<IDirectory> GetExistingSubdirs()
      => this.Info.EnumerateDirectories()
             .Select(subdir => new FinDirectory(subdir));

    public IDirectory GetSubdir(string relativePath, bool create = false)
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

    public bool TryToGetExistingFile(string path, out IFile? file) {
      // TODO: Handle subdirectories automatically.
      var fileInfo = this.Info.GetFiles(path).SingleOrDefault();
      if (fileInfo != null) {
        file = new FinFile(fileInfo);
        return true;
      }

      file = null;
      return false;
    }

    public IFile GetExistingFile(string path) {
      if (TryToGetExistingFile(path, out var file)) {
        return file!;
      }

      throw new Exception(
          $"Expected to find file: '{path}' in directory '{this.GetAbsolutePath()}'");
    }

    public IFile? PossiblyAssertExistingFile(string relativePath, bool assert) {
      if (assert) {
        return GetExistingFile(relativePath);
      }

      TryToGetExistingFile(relativePath, out var file);
      return file;
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

    public bool Equals(IDirectory other)
      => this.GetAbsolutePath() == other.GetAbsolutePath();

    public override int GetHashCode() => this.FullName.GetHashCode();

    public static bool operator ==(FinDirectory lhs, IDirectory rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinDirectory lhs, IDirectory rhs)
      => !lhs.Equals(rhs);
  }
}