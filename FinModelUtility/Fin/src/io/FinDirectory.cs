using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.util.asserts;


namespace fin.io {
  public class FinDirectory : BIoObject, IDirectory {
    public FinDirectory(string fullName) : base(fullName) {}

    public bool Create() {
      if (this.Exists) {
        return false;
      }

      FinFileSystem.Directory.CreateDirectory(this.FullName);
      return true;
    }

    public override bool Exists => FinFileSystem.Directory.Exists(this.FullName);

    public bool Delete(bool recursive = false) {
      if (!this.Exists) {
        return false;
      }

      FinFileSystem.Directory.Delete(this.FullName, recursive);
      return true;
    }

    public void MoveTo(string path) {
      try {
        FinFileSystem.Directory.Move(this.FullName, path);
      }
      // Sometimes the first move throws a permission denied error, so we just need to try again.
      catch {
        FinFileSystem.Directory.Move(this.FullName, path);
      }
    }

    public IEnumerable<IDirectory> GetExistingSubdirs()
      => FinFileSystem.Directory.EnumerateDirectories(this.FullName)
                      .Select(subdir => new FinDirectory(subdir));

    public IDirectory GetSubdir(string relativePath, bool create = false)
      => this.GetSubdirImpl_(relativePath.Split('/', '\\'), create);

    private IDirectory GetSubdirImpl_(
        IEnumerable<string> subdirs,
        bool create) {
      var current = this.FullName;

      foreach (var subdir in subdirs) {
        if (subdir == "") {
          continue;
        }

        if (subdir == "..") {
          current = Asserts.CastNonnull(Path.GetDirectoryName(current));
          continue;
        }

        var matches = FinFileSystem.Directory.GetDirectories(current, subdir);
        if (!create || matches.Length == 1) {
          current = matches.Single();
        } else {
          current = Path.Join(current, subdir);
          FinFileSystem.Directory.CreateDirectory(current);
        }
      }

      return new FinDirectory(current);
    }


    public IEnumerable<IFile> GetExistingFiles()
      => FinFileSystem.Directory.EnumerateFiles(this.FullName)
                      .Select(file => new FinFile(file));

    public IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => FinFileSystem
         .Directory.GetFiles(
             this.FullName,
             searchPattern,
             includeSubdirs
                 ? SearchOption.AllDirectories
                 : SearchOption.TopDirectoryOnly)
         .Select(file => new FinFile(file));

    public bool TryToGetExistingFile(string path, out IFile? file) {
      // TODO: Handle subdirectories automatically.
      var fileInfo = FinFileSystem.Directory.GetFiles(this.FullName, path)
                                  .SingleOrDefault();
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
          $"Expected to find file: '{path}' in directory '{this.FullName}'");
    }

    public IFile? PossiblyAssertExistingFile(string relativePath, bool assert) {
      if (assert) {
        return GetExistingFile(relativePath);
      }

      TryToGetExistingFile(relativePath, out var file);
      return file;
    }

    public IFile[] GetFilesWithExtension(
        string extension,
        bool includeSubdirs = false)
      => FinFileSystem.Directory.GetFiles(
                          this.FullName,
                          $"*{Files.AssertValidExtension(extension)}",
                          includeSubdirs
                              ? SearchOption.AllDirectories
                              : SearchOption.TopDirectoryOnly)
                      .Select(fileInfo => new FinFile(fileInfo))
                      .ToArray();


    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not IDirectory otherDirectory) {
        return false;
      }

      return this.Equals(otherDirectory);
    }

    public bool Equals(IDirectory other) => this.FullName == other.FullName;

    public static bool operator ==(FinDirectory lhs, IDirectory rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinDirectory lhs, IDirectory rhs)
      => !lhs.Equals(rhs);
  }
}