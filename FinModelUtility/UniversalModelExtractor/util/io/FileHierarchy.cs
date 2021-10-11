using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using fin.io;

namespace uni.util.io {
  public interface IFileHierarchy : IEnumerable<IFileHierarchyDirectory> {
    IFileHierarchyDirectory Root { get; }
  }

  public interface IFileHierarchyDirectory {
    public IDirectory Impl { get; }

    public IReadOnlyList<IFileHierarchyDirectory> Subdirs { get; }
    public IReadOnlyList<IFile> Files { get; }

    public bool Refresh(bool recursive = false);
  }

  public class FileHierarchy : IFileHierarchy {
    public FileHierarchy(IDirectory directory) {
      this.Root = new FileHierarchyDirectory(directory);
    }

    public IFileHierarchyDirectory Root { get; }

    private class FileHierarchyDirectory : IFileHierarchyDirectory {
      private List<IFileHierarchyDirectory> subdirs_ = new();
      private List<IFile> files_ = new();

      public FileHierarchyDirectory(IDirectory directory) {
        this.Impl = directory;

        this.Subdirs =
            new ReadOnlyCollection<IFileHierarchyDirectory>(this.subdirs_);
        this.Files =
            new ReadOnlyCollection<IFile>(this.files_);

        this.Refresh();
      }

      public IDirectory Impl { get; }

      public IReadOnlyList<IFileHierarchyDirectory> Subdirs { get; }
      public IReadOnlyList<IFile> Files { get; }

      public bool Refresh(bool recursive = false) {
        var didChange = false;

        var actualSubdirs = this.Impl.GetExistingSubdirs().ToArray();
        foreach (var subdir in this.subdirs_) {
          if (!actualSubdirs.Contains(subdir.Impl)) {
            this.subdirs_.Remove(subdir);
            didChange = true;
          }
        }
        foreach (var actualSubdir in actualSubdirs) {
          if (this.subdirs_.All(subdir => subdir.Impl != actualSubdir)) {
            this.subdirs_.Add(new FileHierarchyDirectory(actualSubdir));
            didChange = true;
          }
        }

        var actualFiles = this.Impl.GetExistingFiles().ToArray();
        foreach (var file in this.files_) {
          if (!actualFiles.Contains(file)) {
            this.files_.Remove(file);
            didChange = true;
          }
        }
        foreach (var actualFile in actualFiles) {
          if (!this.files_.Contains(actualFile)) {
            this.files_.Add(actualFile);
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