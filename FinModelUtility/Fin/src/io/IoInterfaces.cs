using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

using schema;


namespace fin.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }

    bool Exists { get; }

    string? GetParentFullName();
    IDirectory? GetParent();
    IDirectory[]? GetAncestry();
  }

  public interface IDirectory : IIoObject, IEquatable<IDirectory> {
    bool Create();
    bool Delete(bool recursive = false);
    void MoveTo(string path);

    IEnumerable<IDirectory> GetExistingSubdirs();
    IDirectory GetSubdir(string relativePath, bool create = false);

    IEnumerable<IFile> GetExistingFiles();

    IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    bool TryToGetExistingFile(string path, out IFile? file);
    IFile GetExistingFile(string relativePath);
    IFile? PossiblyAssertExistingFile(string relativePath, bool assert);

    IFile[] GetFilesWithExtension(
        string extension,
        bool includeSubdirs = false);
  }

  public interface IFile : IIoObject, IEquatable<IFile> {
    bool Delete();

    string Extension { get; }
    IFile CloneWithExtension(string newExtension);

    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }

    T ReadNew<T>() where T : IDeserializable, new();
    T ReadNew<T>(Endianness endianness) where T : IDeserializable, new();

    byte[] ReadAllBytes();
    string ReadAllText();

    void WriteAllBytes(byte[] bytes);

    StreamReader OpenReadAsText();
    StreamWriter OpenWriteAsText();

    FileSystemStream OpenRead();
    FileSystemStream OpenWrite();

    T Deserialize<T>();
    void Serialize<T>(T instance) where T : notnull;
  }
}