using System.Collections.Generic;

namespace fin.io {
  using GROTreeIoObj =
      IReadOnlyTreeIoObject<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
          IReadOnlyTreeFile, string>;
  using GROTreeDir =
      IReadOnlyTreeDirectory<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
          IReadOnlyTreeFile, string>;
  using GROTreeFile =
      IReadOnlyTreeFile<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
          IReadOnlyTreeFile, string>;
  using GROSysIoObj =
      IReadOnlyTreeIoObject<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using GROSysDir =
      IReadOnlyTreeDirectory<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using GROSysFile =
      IReadOnlyTreeFile<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using GMSysIoObj =
      IReadOnlyTreeIoObject<ISystemIoObject, ISystemDirectory, ISystemFile,
          string>;
  using GMSysDir =
      IReadOnlyTreeDirectory<ISystemIoObject, ISystemDirectory, ISystemFile,
          string>;
  using GMSysFile =
      IReadOnlyTreeFile<ISystemIoObject, ISystemDirectory, ISystemFile, string>;

  // ReadOnly
  public partial interface IReadOnlySystemIoObject {
    // FullPath
    string GROSysIoObj.FullPath => this.FullPath;
    new string FullPath { get; }

    // Name
    string GROSysIoObj.Name => this.Name;
    new string Name { get; }

    // AssertGetParent
    IReadOnlyTreeDirectory GROTreeIoObj.AssertGetParent()
      => this.AssertGetParent();

    new IReadOnlySystemDirectory AssertGetParent();

    // GetAncestry
    IEnumerable<IReadOnlyTreeDirectory> GROTreeIoObj.GetAncestry()
      => this.GetAncestry();

    new IEnumerable<IReadOnlySystemDirectory> GetAncestry();
  }

  public partial interface IReadOnlySystemDirectory {
    // IsEmpty
    new bool IsEmpty { get; }

    // GetExistingSubdirs
    IEnumerable<IReadOnlyTreeDirectory> GROTreeDir.GetExistingSubdirs()
      => this.GetExistingSubdirs();

    IEnumerable<IReadOnlySystemDirectory> GROSysDir.GetExistingSubdirs()
      => this.GetExistingSubdirs();

    new IEnumerable<IReadOnlySystemDirectory> GetExistingSubdirs();

    // AssertGetExistingSubdir
    IReadOnlyTreeDirectory GROTreeDir.AssertGetExistingSubdir(string path)
      => this.AssertGetExistingSubdir(path);

    IReadOnlySystemDirectory GROSysDir.AssertGetExistingSubdir(string path)
      => this.AssertGetExistingSubdir(path);

    new IReadOnlySystemDirectory AssertGetExistingSubdir(string path);

    // TryToGetExistingSubdir
    bool GROTreeDir.TryToGetExistingSubdir(
        string path,
        out IReadOnlyTreeDirectory outDirectory)
      => this.TryToGetExistingSubdir(path, out outDirectory);

    new bool TryToGetExistingSubdir(string path,
                                    out IReadOnlySystemDirectory outDirectory);

    // GetExistingFiles
    IEnumerable<IReadOnlyTreeFile> GROTreeDir.GetExistingFiles()
      => this.GetExistingFiles();

    IEnumerable<IReadOnlySystemFile> GROSysDir.GetExistingFiles()
      => this.GetExistingFiles();

    new IEnumerable<IReadOnlySystemFile> GetExistingFiles();

    // AssertGetExistingFile
    IReadOnlyTreeFile GROTreeDir.AssertGetExistingFile(string path)
      => this.AssertGetExistingFile(path);

    new IReadOnlySystemFile AssertGetExistingFile(string path);

    // TryToGetExistingFile
    bool GROTreeDir.TryToGetExistingFile(string path,
                                         out IReadOnlyTreeFile outFile)
      => this.TryToGetExistingFile(path, out outFile);

    bool GROSysDir.TryToGetExistingFile(string path,
                                        out IReadOnlySystemFile outFile)
      => this.TryToGetExistingFile(path, out outFile);


    new bool TryToGetExistingFile(string path, out IReadOnlySystemFile outFile);

    // SearchForFiles
    IEnumerable<IReadOnlyTreeFile> GROTreeDir.SearchForFiles(
        string searchPattern,
        bool includeSubdirs)
      => this.SearchForFiles(searchPattern, includeSubdirs);

    new IEnumerable<IReadOnlySystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    // GetFilesWithFileType
    IEnumerable<IReadOnlyTreeFile> GROTreeDir.GetFilesWithFileType(
        string fileType,
        bool includeSubdirs)
      => this.GetFilesWithFileType(fileType, includeSubdirs);

    new IEnumerable<IReadOnlySystemFile> GetFilesWithFileType(
        string fileType,
        bool includeSubdirs = false);
  }


  public partial interface IReadOnlySystemFile {
    // FileType
    string GROSysFile.FileType => this.FileType;
    new string FileType { get; }

    // CloneWithFileType
    IReadOnlyTreeFile GROTreeFile.CloneWithFileType(string fileType)
      => this.CloneWithFileType(fileType);

    new IReadOnlySystemFile CloneWithFileType(string fileType);
  }


  // Mutable
  public partial interface ISystemIoObject {
    new string FullPath { get; }
    new string Name { get; }
    new bool Exists { get; }
    new string? GetParentFullPath();

    // AssertGetParent
    IReadOnlySystemDirectory GROSysIoObj.AssertGetParent()
      => this.AssertGetParent();

    IReadOnlySystemDirectory IReadOnlySystemIoObject.AssertGetParent()
      => this.AssertGetParent();

    new ISystemDirectory AssertGetParent();

    // TryGetParent
    bool GROTreeIoObj.TryGetParent(out IReadOnlyTreeDirectory parent)
      => this.TryGetParent(out parent);

    bool GROSysIoObj.TryGetParent(out IReadOnlySystemDirectory parent)
      => this.TryGetParent(out parent);

    new bool TryGetParent(out ISystemDirectory parent);

    // GetAncestry
    IEnumerable<IReadOnlySystemDirectory> GROSysIoObj.GetAncestry()
      => this.GetAncestry();

    IEnumerable<IReadOnlySystemDirectory> IReadOnlySystemIoObject.GetAncestry()
      => this.GetAncestry();

    new IEnumerable<ISystemDirectory> GetAncestry();
  }


  public partial interface ISystemDirectory {
    // Overrides
    new bool IsEmpty { get; }

    // GetExistingSubdirs
    IEnumerable<IReadOnlySystemDirectory> IReadOnlySystemDirectory.
        GetExistingSubdirs() => this.GetExistingSubdirs();

    new IEnumerable<ISystemDirectory> GetExistingSubdirs();

    // AssertGetExistingSubdir
    IReadOnlySystemDirectory IReadOnlySystemDirectory.AssertGetExistingSubdir(
        string path)
      => this.AssertGetExistingSubdir(path);

    new ISystemDirectory AssertGetExistingSubdir(string path);

    new bool TryToGetExistingSubdir(string path,
                                    out ISystemDirectory outDirectory);

    // GetExistingFiles
    IEnumerable<IReadOnlySystemFile>
        IReadOnlySystemDirectory.GetExistingFiles()
      => this.GetExistingFiles();

    new IEnumerable<ISystemFile> GetExistingFiles();

    // AssertGetExistingFile
    IReadOnlySystemFile GROSysDir.AssertGetExistingFile(string path)
      => this.AssertGetExistingFile(path);

    IReadOnlySystemFile IReadOnlySystemDirectory.AssertGetExistingFile(
        string path)
      => this.AssertGetExistingFile(path);

    new ISystemFile AssertGetExistingFile(string path);

    new bool TryToGetExistingFile(string path, out ISystemFile outFile);


    // TryToGetExistingFileWithFileType
    bool IReadOnlySystemDirectory.TryToGetExistingFile(string path,
      out IReadOnlySystemFile outFile)
      => this.TryToGetExistingFile(path, out outFile);

    bool GROTreeDir.TryToGetExistingFileWithFileType(
        string pathWithoutExtension,
        out IReadOnlyTreeFile outFile,
        params string[] fileTypes)
      => this.TryToGetExistingFileWithFileType(
          pathWithoutExtension,
          out outFile,
          fileTypes);

    bool GROSysDir.TryToGetExistingFileWithFileType(
        string pathWithoutExtension,
        out IReadOnlySystemFile outFile,
        params string[] fileTypes)
      => this.TryToGetExistingFileWithFileType(
          pathWithoutExtension,
          out outFile,
          fileTypes);

    new bool TryToGetExistingFileWithFileType(string pathWithoutExtension,
                                              out ISystemFile outFile,
                                              params string[] fileTypes);

    // SearchForFiles
    IEnumerable<IReadOnlySystemFile> IReadOnlySystemDirectory.SearchForFiles(
        string searchPattern,
        bool includeSubdirs)
      => this.SearchForFiles(searchPattern, includeSubdirs);

    IEnumerable<IReadOnlySystemFile> GROSysDir.SearchForFiles(
        string searchPattern,
        bool includeSubdirs)
      => this.SearchForFiles(searchPattern, includeSubdirs);

    new IEnumerable<ISystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    // GetFilesWithFileType
    IEnumerable<IReadOnlySystemFile>
        IReadOnlySystemDirectory.GetFilesWithFileType(
            string fileType,
            bool includeSubdirs)
      => this.GetFilesWithFileType(fileType, includeSubdirs);

    IEnumerable<IReadOnlySystemFile> GROSysDir.GetFilesWithFileType(
        string fileType,
        bool includeSubdirs)
      => this.GetFilesWithFileType(fileType, includeSubdirs);

    new IEnumerable<ISystemFile> GetFilesWithFileType(
        string fileType,
        bool includeSubdirs = false);
  }


  public partial interface ISystemFile {
    // FileType
    string GROSysFile.FileType => this.FileType;
    string IReadOnlySystemFile.FileType => this.FileType;
    new string FileType { get; }

    // CloneWithFileType
    IReadOnlySystemFile GROSysFile.CloneWithFileType(string newFileType)
      => this.CloneWithFileType(newFileType);

    IReadOnlySystemFile IReadOnlySystemFile.CloneWithFileType(
        string newFileType)
      => this.CloneWithFileType(newFileType);

    new ISystemFile CloneWithFileType(string newFileType);
  }
}