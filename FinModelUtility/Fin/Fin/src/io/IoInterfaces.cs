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

  public interface ISystemIoObject : IEquatable<ISystemIoObject> {
    string Name { get; }
    string FullName { get; }

    bool Exists { get; }

    string? GetParentFullName();

    ISystemDirectory GetParent();
    bool TryGetParent(out ISystemDirectory parent);
    ISystemDirectory[] GetAncestry();

    bool Equals(object? other);
  }


  // Directory

  public interface ISystemDirectory : ISystemIoObject {
    bool IsEmpty { get; }

    bool Create();
    
    bool Delete(bool recursive = false);
    bool DeleteContents();

    void MoveTo(string path);

    IEnumerable<ISystemDirectory> GetExistingSubdirs();
    ISystemDirectory GetSubdir(string relativePath, bool create = false);

    IEnumerable<ISystemFile> GetExistingFiles();
    ISystemFile GetExistingFile(string path);
    bool TryToGetExistingFile(string path, out ISystemFile outFile);

    bool TryToGetExistingFileWithExtension(string pathWithoutExtension,
                                           out ISystemFile outFile,
                                           params string[] extensions);
    IEnumerable<ISystemFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    bool PossiblyAssertExistingFile(string relativePath,
                                    bool assert,
                                    out ISystemFile outFile);

    IEnumerable<ISystemFile> GetFilesWithExtension(
        string extension,
        bool includeSubdirs = false);
  }


  // File 
  public interface ISystemFile : ISystemIoObject, IGenericFile {
    bool Delete();

    string Extension { get; }
    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }

    ISystemFile CloneWithExtension(string newExtension);
  }
}