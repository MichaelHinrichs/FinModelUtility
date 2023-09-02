using System.Collections.Generic;

namespace fin.io {
  using IROTreeIoObj =
      IReadOnlyTreeIoObject<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using IROTreeDir =
      IReadOnlyTreeDirectory<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using IROTreeFile =
      IReadOnlyTreeFile<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using ITreeIoObj =
      IReadOnlyTreeIoObject<ISystemIoObject, ISystemDirectory, ISystemFile,
          string>;
  using ITreeDir =
      IReadOnlyTreeDirectory<ISystemIoObject, ISystemDirectory, ISystemFile,
          string>;
  using ITreeFile =
      IReadOnlyTreeFile<ISystemIoObject, ISystemDirectory, ISystemFile, string>;

  // ReadOnly
  public interface IReadOnlySystemIoObject : IROTreeIoObj {
    bool Exists { get; }

    string? GetParentFullPath();
  }

  public interface IReadOnlySystemDirectory
      : IReadOnlySystemIoObject,
        IROTreeDir { }


  public interface IReadOnlySystemFile
      : IReadOnlySystemIoObject,
        IROTreeFile {
    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }
  }


  // Mutable

  public interface ISystemIoObject : IReadOnlySystemIoObject, ITreeIoObj {
    new string FullPath { get; }
    new string Name { get; }
    new bool Exists { get; }
    new string? GetParentFullPath();
    new ISystemDirectory AssertGetParent();
    new bool TryGetParent(out ISystemDirectory parent);
    new IEnumerable<ISystemDirectory> GetAncestry();
  }


  public interface ISystemDirectory
      : ISystemIoObject, IReadOnlySystemDirectory, ITreeDir {
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


    IEnumerable<IReadOnlySystemFile> IROTreeDir.GetFilesWithFileType(
        string fileType,
        bool includeSubdirs = false)
      => this.GetFilesWithFileType(fileType, includeSubdirs);

    new IEnumerable<ISystemFile> GetFilesWithFileType(
        string fileType,
        bool includeSubdirs = false);
  }


  public interface ISystemFile
      : ISystemIoObject, IReadOnlySystemFile, IGenericFile, ITreeFile {
    bool Delete();


    string IROTreeFile.FileType => this.FileType;
    new string FileType { get; }


    IReadOnlySystemFile IROTreeFile.CloneWithFileType(string newFileType)
      => this.CloneWithFileType(newFileType);

    new ISystemFile CloneWithFileType(string newFileType);
  }
}