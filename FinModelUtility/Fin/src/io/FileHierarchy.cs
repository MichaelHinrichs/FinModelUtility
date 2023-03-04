using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using fin.util.asserts;
using fin.util.data;
using fin.util.linq;

using fins.io.sharpDirLister;


namespace fin.io {
  public interface IFileHierarchy : IEnumerable<IFileHierarchyDirectory> {
    IFileHierarchyDirectory Root { get; }
  }

  public interface IFileHierarchyInstance {
    IFileHierarchyDirectory Root { get; }
    IFileHierarchyDirectory? Parent { get; }
    bool Exists { get; }

    string FullName { get; }
    string Name { get; }

    string LocalPath { get; }
  }

  public interface IFileHierarchyDirectory : IFileHierarchyInstance {
    IDirectory Impl { get; }

    IReadOnlyList<IFileHierarchyDirectory> Subdirs { get; }
    IReadOnlyList<IFileHierarchyFile> Files { get; }

    bool Refresh(bool recursive = false);

    IFileHierarchyFile GetExistingFile(string localPath);
    IFileHierarchyDirectory GetExistingSubdir(string localPath);

    bool TryToGetExistingSubdir(string localPath,
                                out IFileHierarchyDirectory outDirectory);

    IEnumerable<IFileHierarchyFile> FilesWithExtension(string extension);

    IEnumerable<IFileHierarchyFile> FilesWithExtensions(
        IEnumerable<string> extensions);

    IEnumerable<IFileHierarchyFile> FilesWithExtensions(
        string first,
        params string[] rest);

    IEnumerable<IFileHierarchyFile> FilesWithExtensionRecursive(
        string extension);

    IEnumerable<IFileHierarchyFile> FilesWithExtensionsRecursive(
        IEnumerable<string> extensions);

    IEnumerable<IFileHierarchyFile> FilesWithExtensionsRecursive(
        string first,
        params string[] rest);
  }

  public interface IFileHierarchyFile : IFileHierarchyInstance {
    IFile Impl { get; }

    string Extension { get; }

    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }
  }


  public class FileHierarchy : IFileHierarchy {
    public FileHierarchy(IDirectory directory) {
      var populatedSubdirs =
          SharpFileLister.FindNextFilePInvokeRecursiveParalleled(
              directory.FullName);
      this.Root = new FileHierarchyDirectory(directory,
                                             populatedSubdirs);
    }

    public IFileHierarchyDirectory Root { get; }

    private class FileHierarchyDirectory : IFileHierarchyDirectory {
      private readonly IDirectory baseDirectory_;

      private List<IFileHierarchyDirectory> subdirs_ = new();
      private List<IFileHierarchyFile> files_ = new();

      public FileHierarchyDirectory(
          IDirectory directory,
          ISubdirPaths paths) {
        this.Root = this;
        this.baseDirectory_ = this.Impl = directory;
        this.LocalPath = "";

        this.Subdirs =
            new ReadOnlyCollection<IFileHierarchyDirectory>(this.subdirs_);
        this.Files =
            new ReadOnlyCollection<IFileHierarchyFile>(this.files_);

        foreach (var filePath in paths.AbsoluteFilePaths) {
          this.files_.Add(
              new FileHierarchyFile(this,
                                    this,
                                    new FinFile(filePath),
                                    directory));
        }

        foreach (var subdir in paths.Subdirs) {
          this.subdirs_.Add(
              new FileHierarchyDirectory(
                  this,
                  this,
                  new FinDirectory(subdir.AbsoluteSubdirPath),
                  directory,
                  subdir));
        }
      }

      private FileHierarchyDirectory(
          IFileHierarchyDirectory root,
          IFileHierarchyDirectory parent,
          IDirectory directory,
          IDirectory baseDirectory,
          ISubdirPaths paths) {
        this.baseDirectory_ = baseDirectory;

        this.Root = root;
        this.Parent = parent;
        this.Impl = directory;
        this.LocalPath =
            directory.FullName.Substring(baseDirectory.FullName.Length);

        this.Subdirs =
            new ReadOnlyCollection<IFileHierarchyDirectory>(this.subdirs_);
        this.Files =
            new ReadOnlyCollection<IFileHierarchyFile>(this.files_);

        foreach (var filePath in paths.AbsoluteFilePaths) {
          this.files_.Add(
              new FileHierarchyFile(root,
                                    this,
                                    new FinFile(filePath),
                                    baseDirectory));
        }

        foreach (var subdir in paths.Subdirs) {
          this.subdirs_.Add(
              new FileHierarchyDirectory(
                  root,
                  this,
                  new FinDirectory(subdir.AbsoluteSubdirPath),
                  baseDirectory,
                  subdir));
        }
      }

      private FileHierarchyDirectory(
          IFileHierarchyDirectory root,
          IFileHierarchyDirectory parent,
          IDirectory directory,
          IDirectory baseDirectory) {
        this.baseDirectory_ = baseDirectory;

        this.Root = root;
        this.Parent = parent;
        this.Impl = directory;
        this.LocalPath =
            directory.FullName.Substring(baseDirectory.FullName.Length);

        this.Subdirs =
            new ReadOnlyCollection<IFileHierarchyDirectory>(this.subdirs_);
        this.Files =
            new ReadOnlyCollection<IFileHierarchyFile>(this.files_);

        this.Refresh();
      }

      public IFileHierarchyDirectory Root { get; }
      public IFileHierarchyDirectory? Parent { get; }

      public IDirectory Impl { get; }


      public bool Exists => this.Impl.Exists;

      public string FullName => this.Impl.FullName;
      public string Name => this.Impl.Name;

      public string LocalPath { get; }


      public IReadOnlyList<IFileHierarchyDirectory> Subdirs { get; }
      public IReadOnlyList<IFileHierarchyFile> Files { get; }

      public bool Refresh(bool recursive = false) {
        var didChange = false;

        var actualSubdirs = this.Impl.GetExistingSubdirs()
                                .CastTo<FinDirectory, IDirectory>()
                                .ToArray();
        didChange |=
            ListUtil.RemoveWhere(this.subdirs_,
                                 subdir => !actualSubdirs
                                     .Contains(subdir.Impl));
        foreach (var actualSubdir in actualSubdirs) {
          if (this.subdirs_.All(subdir => !subdir.Impl.Equals(actualSubdir))) {
            this.subdirs_.Add(
                new FileHierarchyDirectory(this.Root,
                                           this,
                                           actualSubdir,
                                           this.baseDirectory_));
            didChange = true;
          }
        }

        var actualFiles = this.Impl.GetExistingFiles()
                              .CastTo<FinFile, IFile>()
                              .ToArray();
        didChange |=
            ListUtil.RemoveWhere(this.files_,
                                 file => !actualFiles.Contains(file.Impl));
        foreach (var actualFile in actualFiles) {
          if (this.files_.All(file => !file.Impl.Equals(actualFile))) {
            this.files_.Add(
                new FileHierarchyFile(this.Root,
                                      this,
                                      actualFile,
                                      this.baseDirectory_));
            didChange = true;
          }
        }

        if (recursive) {
          foreach (var subdir in this.subdirs_) {
            didChange |= subdir.Refresh(true);
          }
        }

        return didChange;
      }

      public IFileHierarchyFile GetExistingFile(string relativePath) {
        var subdirs = relativePath.Split('/', '\\');

        IFileHierarchyDirectory current = this;
        foreach (var subdir in subdirs.SkipLast(1)) {
          if (subdir == "") {
            continue;
          }

          if (subdir == "..") {
            Asserts.Fail();
            continue;
          }

          current = current.Subdirs
                           .Single(dir => dir.Name == subdir);
        }

        return current.Files.Single(file => file.Name == subdirs.Last());
      }

      public IFileHierarchyDirectory GetExistingSubdir(string relativePath) {
        var subdirs = relativePath.Split('/', '\\');

        IFileHierarchyDirectory current = this;
        foreach (var subdir in subdirs) {
          if (subdir == "") {
            continue;
          }

          if (subdir == "..") {
            Asserts.Fail();
            continue;
          }

          current = current.Subdirs
                           .Single(dir => dir.Name == subdir);
        }

        return current;
      }

      public bool TryToGetExistingSubdir(
          string localPath,
          out IFileHierarchyDirectory outDirectory) {
        try {
          outDirectory = this.GetExistingSubdir(localPath);
          return true;
        } catch {
          outDirectory = null;
          return false;
        }
      }


      public IEnumerable<IFileHierarchyFile> FilesWithExtension(
          string extension)
        => this.Files.Where(file => file.Extension == extension);

      public IEnumerable<IFileHierarchyFile> FilesWithExtensions(
          IEnumerable<string> extensions)
        => this.Files.Where(file => extensions.Contains(file.Extension));

      public IEnumerable<IFileHierarchyFile> FilesWithExtensions(
          string first,
          params string[] rest)
        => this.Files.Where(file => file.Extension == first ||
                                    rest.Contains(file.Extension));

      public IEnumerable<IFileHierarchyFile> FilesWithExtensionRecursive(
          string extension)
        => this.FilesWithExtension(extension)
               .Concat(
                   this.Subdirs.SelectMany(
                       subdir
                           => subdir.FilesWithExtensionRecursive(extension)));

      public IEnumerable<IFileHierarchyFile> FilesWithExtensionsRecursive(
          IEnumerable<string> extensions)
        => this.FilesWithExtensions(extensions)
               .Concat(
                   this.Subdirs.SelectMany(
                       subdir
                           => subdir.FilesWithExtensionsRecursive(extensions)));

      public IEnumerable<IFileHierarchyFile> FilesWithExtensionsRecursive(
          string first,
          params string[] rest)
        => this.FilesWithExtensions(first, rest)
               .Concat(
                   this.Subdirs.SelectMany(
                       subdir
                           => subdir
                               .FilesWithExtensionsRecursive(first, rest)));

      public override string ToString() => this.LocalPath;
    }

    private class FileHierarchyFile : IFileHierarchyFile {
      public FileHierarchyFile(IFileHierarchyDirectory root,
                               IFileHierarchyDirectory parent,
                               IFile? file,
                               IDirectory baseDirectory) {
        this.Root = root;
        this.Parent = parent;
        this.Impl = file;
        this.LocalPath =
            file.FullName.Substring(baseDirectory.FullName.Length);
      }

      public IFileHierarchyDirectory Root { get; }
      public IFileHierarchyDirectory Parent { get; }

      public IFile Impl { get; }

      public bool Exists => this.Impl.Exists;

      public string FullName => this.Impl.FullName;
      public string Name => this.Impl.Name;

      public string FullNameWithoutExtension
        => this.Impl.FullNameWithoutExtension;

      public string NameWithoutExtension => this.Impl.NameWithoutExtension;

      public string Extension => this.Impl.Extension;

      public string LocalPath { get; }

      public override string ToString() => this.LocalPath;
    }

    public IEnumerator<IFileHierarchyDirectory> GetEnumerator() {
      var directoryQueue = new Queue<IFileHierarchyDirectory>();
      directoryQueue.Enqueue(this.Root);
      while (directoryQueue.Count > 0) {
        var directory = directoryQueue.Dequeue();

        yield return directory;

        foreach (var subdir in directory.Subdirs) {
          directoryQueue.Enqueue(subdir);
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }
}