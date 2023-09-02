using System;
using System.Collections.Generic;

namespace fin.io {
  // TODO: Come up with a better name for these "tree" interfaces?
  // The idea is that:
  // - generic files are just standalone files, don't necessarily have parents
  //   - can be readonly or mutable
  // - "tree" files are files that exist in a hierarchy, these may be within a file system or an archive
  //   - due to the ambiguity, these are always readonly
  // - system files refer to real files that exist within the file system
  //   - these can be readonly or mutable

  public interface IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile,
                                         TFileType>
      : IEquatable<TIoObject>
      where TIoObject :
      IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      IReadOnlyTreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : IReadOnlyTreeFile<TIoObject, TDirectory, TFile, TFileType> {
    string FullPath { get; }
    string Name { get; }

    TDirectory AssertGetParent();
    bool TryGetParent(out TDirectory parent);
    IEnumerable<TDirectory> GetAncestry();
  }

  public interface IReadOnlyTreeDirectory<TIoObject, TDirectory, TFile,
                                          TFileType>
      : IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TIoObject :
      IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      IReadOnlyTreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : IReadOnlyTreeFile<TIoObject, TDirectory, TFile, TFileType> {
    bool IsEmpty { get; }

    IEnumerable<TDirectory> GetExistingSubdirs();
    TDirectory AssertGetExistingSubdir(string path);
    bool TryToGetExistingSubdir(string path, out TDirectory outDirectory);

    IEnumerable<TFile> GetExistingFiles();
    TFile AssertGetExistingFile(string path);
    bool TryToGetExistingFile(string path, out TFile outFile);

    bool TryToGetExistingFileWithFileType(string pathWithoutExtension,
                                          out TFile outFile,
                                          params TFileType[] fileTypes);

    IEnumerable<TFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    IEnumerable<TFile> GetFilesWithFileType(
        TFileType fileType,
        bool includeSubdirs = false);
  }

  public interface IReadOnlyTreeFile<TIoObject, TDirectory, TFile, TFileType>
      : IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile, TFileType>,
        IReadOnlyGenericFile
      where TIoObject :
      IReadOnlyTreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      IReadOnlyTreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : IReadOnlyTreeFile<TIoObject, TDirectory, TFile, TFileType> {
    TFileType FileType { get; }
  }


  public interface IReadOnlyTreeIoObject
      : IReadOnlyTreeIoObject<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
          IReadOnlyTreeFile, string> { }

  public interface IReadOnlyTreeDirectory
      : IReadOnlyTreeIoObject,
        IReadOnlyTreeDirectory<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
            IReadOnlyTreeFile, string> { }

  public interface IReadOnlyTreeFile
      : IReadOnlyTreeIoObject,
        IReadOnlyTreeFile<IReadOnlyTreeIoObject, IReadOnlyTreeDirectory,
            IReadOnlyTreeFile, string> { }
}