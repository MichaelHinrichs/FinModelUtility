using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace fin.io {
  public readonly struct FinDirectory : ISystemDirectory {
    public FinDirectory(string fullName) {
      this.FullName = fullName;
    }

    public override string ToString() => this.DisplayFullName;


    // Equality
    public bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not ISystemIoObject otherSelf) {
        return false;
      }

      return this.Equals(otherSelf);
    }

    public bool Equals(ISystemIoObject? other)
      => this.FullName == other?.FullName;


    // Directory fields
    public string Name => FinIoStatic.GetName(this.FullName);
    public string FullName { get; }
    public string DisplayFullName => this.FullName;


    // Ancestry methods
    public string? GetParentFullName()
      => FinIoStatic.GetParentFullName(this.FullName);

    public ISystemDirectory GetParent() {
      if (this.TryGetParent(out var parent)) {
        return parent;
      }

      throw new Exception("Expected parent directory to exist!");
    }

    public bool TryGetParent(out ISystemDirectory parent) {
      var parentName = this.GetParentFullName();
      if (parentName != null) {
        parent = new FinDirectory(parentName);
        return true;
      }

      parent = default;
      return false;
    }

    public ISystemDirectory[] GetAncestry() {
      if (!this.TryGetParent(out var firstParent)) {
        return Array.Empty<ISystemDirectory>();
      }

      var parents = new LinkedList<ISystemDirectory>();
      var current = firstParent;
      while (current.TryGetParent(out var parent)) {
        parents.AddLast(parent);
        current = parent;
      }

      return parents.ToArray();
    }


    // Directory methods
    public bool IsEmpty => FinDirectoryStatic.IsEmpty(this.FullName);
    public bool Exists => FinDirectoryStatic.Exists(this.FullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Create() => FinDirectoryStatic.Create(this.FullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Delete(bool recursive = false)
      => FinDirectoryStatic.Delete(this.FullName, recursive);

    public bool DeleteContents() {
      var didDeleteAnything = false;
      foreach (var file in this.GetExistingFiles()) {
        didDeleteAnything |= file.Delete();
      }

      foreach (var directory in this.GetExistingSubdirs()) {
        didDeleteAnything |= directory.Delete(true);
      }

      return didDeleteAnything;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveTo(string path)
      => FinDirectoryStatic.MoveTo(this.FullName, path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemDirectory> GetExistingSubdirs()
      => FinDirectoryStatic
         .GetExistingSubdirs(this.FullName)
         .Select(fullName => (ISystemDirectory) new FinDirectory(fullName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISystemDirectory GetSubdir(string relativePath, bool create = false)
      => new FinDirectory(
          FinDirectoryStatic.GetSubdir(this.FullName, relativePath, create));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> GetExistingFiles()
      => FinDirectoryStatic.GetExistingFiles(this.FullName)
                           .Select(fullName
                                       => (ISystemFile) new FinFile(fullName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => FinDirectoryStatic
         .SearchForFiles(this.FullName, searchPattern, includeSubdirs)
         .Select(fullName => (ISystemFile) new FinFile(fullName));

    public bool TryToGetExistingFile(string path, out ISystemFile outFile) {
      if (FinDirectoryStatic.TryToGetExistingFile(
              this.FullName,
              path,
              out var fileFullName)) {
        outFile = new FinFile(fileFullName);
        return true;
      }

      outFile = default;
      return false;
    }

    public bool TryToGetExistingFileWithExtension(string pathWithoutExtension,
                                                  out ISystemFile outFile,
                                                  params string[] extensions) {
      foreach (var extension in extensions) {
        if (this.TryToGetExistingFile($"{pathWithoutExtension}{extension}",
                                      out outFile)) {
          return true;
        }
      }

      outFile = default;
      return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISystemFile GetExistingFile(string path)
      => new FinFile(FinDirectoryStatic.GetExistingFile(this.FullName, path));

    public bool PossiblyAssertExistingFile(string relativePath,
                                           bool assert,
                                           out ISystemFile outFile) {
      var fileFullName =
          FinDirectoryStatic.PossiblyAssertExistingFile(
              this.FullName,
              relativePath,
              assert);
      if (fileFullName != null) {
        outFile = new FinFile(fileFullName);
        return true;
      }

      outFile = default;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> GetFilesWithExtension(
        string extension,
        bool includeSubdirs = false)
      => FinDirectoryStatic
         .GetFilesWithExtension(this.FullName, extension, includeSubdirs)
         .Select(fullName => (ISystemFile) new FinFile(fullName));
  }
}