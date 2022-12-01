using System;
using System.Collections.Generic;
using System.IO;

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
    DirectoryInfo Info { get; }

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
    FileInfo Info { get; }

    string Extension { get; }
    IFile CloneWithExtension(string newExtension);

    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }

    T ReadNew<T>() where T : IDeserializable, new();
    T ReadNew<T>(Endianness endianness) where T : IDeserializable, new();

    byte[] ReadAllBytes();
    string ReadAllText();

    StreamReader OpenReadAsText();

    FileStream OpenRead();
    FileStream OpenWrite();

    T Deserialize<T>();
    void Serialize<T>(T instance) where T : notnull;
  }
}