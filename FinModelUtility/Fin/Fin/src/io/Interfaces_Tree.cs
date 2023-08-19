using System;
using System.Collections.Generic;

namespace fin.io {
  public interface ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      : IEquatable<TIoObject>
      where TIoObject :
      ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : ITreeFile<TIoObject, TDirectory, TFile, TFileType> {
    string FullPath { get; }
    string Name { get; }

    TDirectory AssertGetParent();
    bool TryGetParent(out TDirectory parent);
    IEnumerable<TDirectory> GetAncestry();
  }

  public interface ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      : ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TIoObject :
      ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : ITreeFile<TIoObject, TDirectory, TFile, TFileType> {
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

    // TODO: Delete this method
    bool PossiblyAssertExistingFile(string relativePath,
                                    bool assert,
                                    out TFile outFile);

    IEnumerable<TFile> GetFilesWithFileType(
        TFileType fileType,
        bool includeSubdirs = false);
  }

  public interface ITreeFile<TIoObject, TDirectory, TFile, TFileType>
      : ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>,
        IReadOnlyGenericFile
      where TIoObject :
      ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : ITreeFile<TIoObject, TDirectory, TFile, TFileType> {
    TFileType FileType { get; }
    TFile CloneWithFileType(string newFileType);
  }
}