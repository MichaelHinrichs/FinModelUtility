using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

using schema;


namespace fin.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }
    string GetAbsolutePath();

    bool Exists { get; }

    IDirectory? GetParent();
    IDirectory[]? GetAncestry();
  }

  public interface IDirectory : IIoObject, IEquatable<IDirectory> {
    IDirectoryInfo Info { get; }

    bool Create();
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
  }

  public interface IFile : IIoObject, IEquatable<IFile> {
    IFileInfo Info { get; }

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