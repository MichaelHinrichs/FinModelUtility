using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

using schema.binary;

namespace fin.io {
  public interface IReadOnlyGenericFile {
    string DisplayPath { get; }

    FileSystemStream OpenRead();
    StreamReader OpenReadAsText();

    T ReadNew<T>() where T : IBinaryDeserializable, new();
    T ReadNew<T>(Endianness endianness) where T : IBinaryDeserializable, new();

    byte[] ReadAllBytes();
    string ReadAllText();
    T Deserialize<T>();
  }

  public interface IGenericFile : IReadOnlyGenericFile {
    FileSystemStream OpenWrite();
    StreamWriter OpenWriteAsText();

    void WriteAllBytes(ReadOnlyMemory<byte> bytes);
    void WriteAllText(string text);

    void Serialize<T>(T instance) where T : notnull;
  }


  public interface ITreeIoObject<TIoObject, out TDirectory, TFile, TFileType>
      : IEquatable<TIoObject>
      where TIoObject :
      ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : ITreeFile<TIoObject, TDirectory, TFile, TFileType> {
    string FullPath { get; }
    string Name { get; }

    TDirectory AssertGetParent();
    bool TryGetParent(out ISystemDirectory parent);
    
    ISystemDirectory[] GetAncestry();
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

  public interface ITreeFile<TIoObject, out TDirectory, TFile, TFileType>
      : ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>,
        IGenericFile
      where TIoObject :
      ITreeIoObject<TIoObject, TDirectory, TFile, TFileType>
      where TDirectory :
      ITreeDirectory<TIoObject, TDirectory, TFile, TFileType>
      where TFile : ITreeFile<TIoObject, TDirectory, TFile, TFileType> {
    TFileType FileType { get; }
    TFile CloneWithFileType(string newFileType);
  }


  public interface ISystemIoObject
      : ITreeIoObject<ISystemIoObject, ISystemDirectory, ISystemFile,
          string> {
    bool Exists { get; }

    string? GetParentFullPath();
  }


  // Directory

  public interface ISystemDirectory
      : ISystemIoObject,
        ITreeDirectory<ISystemIoObject, ISystemDirectory, ISystemFile,
            string> {
    bool Create();

    bool Delete(bool recursive = false);
    bool DeleteContents();

    void MoveTo(string path);

    ISystemDirectory GetOrCreateSubdir(string relativePath);
  }


  // File 
  public interface ISystemFile
      : ISystemIoObject,
        ITreeFile<ISystemIoObject, ISystemDirectory, ISystemFile, string> {
    bool Delete();

    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }
  }
}