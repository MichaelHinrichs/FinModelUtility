using System.Collections.Generic;

namespace fin.io {
  public interface IFileHierarchy : IEnumerable<IFileHierarchyDirectory> {
    IFileHierarchyDirectory Root { get; }
  }

  public partial interface IFileHierarchyIoObject : IReadOnlyTreeIoObject {
    string LocalPath { get; }

    IFileHierarchyDirectory Root { get; }
    IFileHierarchyDirectory? Parent { get; }
    bool Exists { get; }
  }

  public partial interface IFileHierarchyDirectory
      : IFileHierarchyIoObject, IReadOnlyTreeDirectory {
    ISystemDirectory Impl { get; }

    bool Refresh(bool recursive = false);

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

  public partial interface IFileHierarchyFile
      : IFileHierarchyIoObject, IReadOnlyTreeFile {
    ISystemFile Impl { get; }
  }
}