using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using fin.util.asserts;

namespace fin.io {
  using IROTreeIoObj =
      ITreeIoObject<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using IROTreeDir =
      ITreeDirectory<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;

  public readonly struct FinDirectory : ISystemDirectory {
    public FinDirectory(string fullName) {
      this.FullPath = fullName;
    }

    public override string ToString() => this.DisplayFullName;


    // Equality
    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not ISystemIoObject otherSelf) {
        return false;
      }

      return this.Equals(otherSelf);
    }

    public bool Equals(ISystemIoObject? other)
      => this.Equals(other as IReadOnlySystemIoObject);

    public bool Equals(IReadOnlySystemIoObject? other)
      => this.FullPath == other?.FullPath;


    // Directory fields
    public string Name => FinIoStatic.GetName(this.FullPath);
    public string FullPath { get; }
    public string DisplayFullName => this.FullPath;


    // Ancestry methods
    public string? GetParentFullPath()
      => FinIoStatic.GetParentFullName(this.FullPath);

    IReadOnlySystemDirectory IROTreeIoObj.AssertGetParent()
      => this.AssertGetParent();

    public ISystemDirectory AssertGetParent() {
      if (this.TryGetParent(out ISystemDirectory parent)) {
        return parent;
      }

      throw new Exception("Expected parent directory to exist!");
    }

    public bool TryGetParent(out IReadOnlySystemDirectory parent) {
      parent = default;
      return this.TryGetParent(
          out Unsafe
              .As<IReadOnlySystemDirectory, ISystemDirectory>(ref parent));
    }

    public bool TryGetParent(out ISystemDirectory parent) {
      var parentName = this.GetParentFullPath();
      if (parentName != null) {
        parent = new FinDirectory(parentName);
        return true;
      }

      parent = default;
      return false;
    }

    IEnumerable<IReadOnlySystemDirectory> IROTreeIoObj.GetAncestry()
      => this.GetAncestry();

    public IEnumerable<ISystemDirectory> GetAncestry()
      => this.GetUpwardAncestry_().Reverse();

    private IEnumerable<ISystemDirectory> GetUpwardAncestry_() {
      if (!this.TryGetParent(out ISystemDirectory firstParent)) {
        yield break;
      }

      var current = firstParent;
      while (current.TryGetParent(out var parent)) {
        yield return parent;
        current = parent;
      }
    }


    // Directory methods
    public bool IsEmpty => FinDirectoryStatic.IsEmpty(this.FullPath);
    public bool Exists => FinDirectoryStatic.Exists(this.FullPath);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Create() => FinDirectoryStatic.Create(this.FullPath);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Delete(bool recursive = false)
      => FinDirectoryStatic.Delete(this.FullPath, recursive);

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
      => FinDirectoryStatic.MoveTo(this.FullPath, path);


    IEnumerable<IReadOnlySystemDirectory> IROTreeDir.GetExistingSubdirs()
      => this.GetExistingSubdirs();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemDirectory> GetExistingSubdirs()
      => FinDirectoryStatic
         .GetExistingSubdirs(this.FullPath)
         .Select(fullName => (ISystemDirectory) new FinDirectory(fullName));

    public bool TryToGetExistingSubdir(
        string path,
        out IReadOnlySystemDirectory outDirectory) {
      outDirectory = default;
      return TryToGetExistingSubdir(
          path,
          out Unsafe.As<IReadOnlySystemDirectory, ISystemDirectory>(
              ref outDirectory));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryToGetExistingSubdir(string relativePath,
                                       out ISystemDirectory subdir) {
      subdir = new FinDirectory(
          FinDirectoryStatic.GetSubdir(this.FullPath, relativePath));
      return subdir.Exists;
    }

    IReadOnlySystemDirectory IROTreeDir.AssertGetExistingSubdir(string path)
      => this.AssertGetExistingSubdir(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISystemDirectory AssertGetExistingSubdir(string relativePath) {
      Asserts.True(
          this.TryToGetExistingSubdir(relativePath,
                                      out ISystemDirectory subdir));
      return subdir;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISystemDirectory GetOrCreateSubdir(string relativePath)
      => new FinDirectory(
          FinDirectoryStatic.GetSubdir(this.FullPath, relativePath, true));


    IEnumerable<IReadOnlySystemFile> IROTreeDir.GetExistingFiles()
      => this.GetExistingFiles();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> GetExistingFiles()
      => FinDirectoryStatic.GetExistingFiles(this.FullPath)
                           .Select(fullName
                                       => (ISystemFile) new FinFile(fullName));

    IEnumerable<IReadOnlySystemFile> IROTreeDir.SearchForFiles(
        string searchPattern,
        bool includeSubdirs)
      => this.SearchForFiles(searchPattern, includeSubdirs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => FinDirectoryStatic
         .SearchForFiles(this.FullPath, searchPattern, includeSubdirs)
         .Select(fullName => (ISystemFile) new FinFile(fullName));

    public bool TryToGetExistingFile(string path,
                                     out IReadOnlySystemFile outFile) {
      outFile = default;
      return TryToGetExistingFile(
          path,
          out Unsafe.As<IReadOnlySystemFile, ISystemFile>(ref outFile));
    }

    public bool TryToGetExistingFile(string path, out ISystemFile outFile) {
      if (FinDirectoryStatic.TryToGetExistingFile(
              this.FullPath,
              path,
              out var fileFullName)) {
        outFile = new FinFile(fileFullName);
        return true;
      }

      outFile = default;
      return false;
    }

    public bool TryToGetExistingFileWithFileType(
        string pathWithoutExtension,
        out IReadOnlySystemFile outFile,
        params string[] fileTypes) {
      outFile = default;
      return TryToGetExistingFileWithFileType(
          pathWithoutExtension,
          out Unsafe.As<IReadOnlySystemFile, ISystemFile>(ref outFile),
          fileTypes);
    }

    public bool TryToGetExistingFileWithFileType(string pathWithoutExtension,
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

    IReadOnlySystemFile IROTreeDir.AssertGetExistingFile(string path)
      => this.AssertGetExistingFile(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISystemFile AssertGetExistingFile(string path)
      => new FinFile(FinDirectoryStatic.GetExistingFile(this.FullPath, path));

    public bool PossiblyAssertExistingFile(string relativePath,
                                           bool assert,
                                           out IReadOnlySystemFile outFile) {
      outFile = default;
      return this.PossiblyAssertExistingFile(
          relativePath,
          assert,
          out Unsafe.As<IReadOnlySystemFile, ISystemFile>(ref outFile));
    }

    public bool PossiblyAssertExistingFile(string relativePath,
                                           bool assert,
                                           out ISystemFile outFile) {
      var fileFullName =
          FinDirectoryStatic.PossiblyAssertExistingFile(
              this.FullPath,
              relativePath,
              assert);
      if (fileFullName != null) {
        outFile = new FinFile(fileFullName);
        return true;
      }

      outFile = default;
      return false;
    }

    IEnumerable<IReadOnlySystemFile> IROTreeDir.GetFilesWithFileType(
            string fileType,
            bool includeSubdirs)
      => this.GetFilesWithFileType(fileType, includeSubdirs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ISystemFile> GetFilesWithFileType(
        string extension,
        bool includeSubdirs = false)
      => FinDirectoryStatic
         .GetFilesWithExtension(this.FullPath, extension, includeSubdirs)
         .Select(fullName => (ISystemFile) new FinFile(fullName));
  }
}