using System.Collections.Generic;

namespace fin.io {
  // ReadOnly
  public interface IReadOnlySystemIoObject
      : ITreeIoObject<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile,
          string> {
    bool Exists { get; }

    string? GetParentFullPath();
  }

  public interface IReadOnlySystemDirectory
      : IReadOnlySystemIoObject,
        ITreeDirectory<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
            IReadOnlySystemFile,
            string> { }

  public interface IReadOnlySystemFile
      : IReadOnlySystemIoObject,
        ITreeFile<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
            IReadOnlySystemFile, string> {
    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }
  }


  // Mutable
  public interface ISystemIoObject
      : IReadOnlySystemIoObject,
        ITreeIoObject<ISystemIoObject, ISystemDirectory, ISystemFile,
            string> {
    new string FullPath { get; }
    new string Name { get; }
    new bool Exists { get; }
    new string? GetParentFullPath();
    new ISystemDirectory AssertGetParent();
    new bool TryGetParent(out ISystemDirectory parent);
    new IEnumerable<ISystemDirectory> GetAncestry();
  }

  public interface ISystemDirectory
      : ISystemIoObject,
        IReadOnlySystemDirectory,
        ITreeDirectory<ISystemIoObject, ISystemDirectory, ISystemFile,
            string> {
    bool Create();

    bool Delete(bool recursive = false);
    bool DeleteContents();

    void MoveTo(string path);

    ISystemDirectory GetOrCreateSubdir(string relativePath);


    // Overrides
    new bool IsEmpty { get; }
    new IEnumerable<ISystemDirectory> GetExistingSubdirs();
    new ISystemDirectory AssertGetExistingSubdir(string path);

    new bool TryToGetExistingSubdir(string path,
                                    out ISystemDirectory outDirectory);

    new IEnumerable<ISystemFile> GetExistingFiles();
    new ISystemFile AssertGetExistingFile(string path);
    new bool TryToGetExistingFile(string path, out ISystemFile outFile);

    new bool TryToGetExistingFileWithFileType(string pathWithoutExtension,
                                              out ISystemFile outFile,
                                              params string[] fileTypes);

    new IEnumerable<ISystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    // TODO: Delete this method
    new bool PossiblyAssertExistingFile(string relativePath,
                                        bool assert,
                                        out ISystemFile outFile);

    new IEnumerable<ISystemFile> GetFilesWithFileType(
        string fileType,
        bool includeSubdirs = false);
  }

  public interface ISystemFile
      : ISystemIoObject,
        IReadOnlySystemFile,
        IGenericFile,
        ITreeFile<ISystemIoObject, ISystemDirectory, ISystemFile, string> {
    bool Delete();

    new string FileType { get; }
    new ISystemFile CloneWithFileType(string newFileType);
  }
}