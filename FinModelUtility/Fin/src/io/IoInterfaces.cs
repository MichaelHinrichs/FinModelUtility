using System;
using System.Collections.Generic;
using System.IO;

namespace fin.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }

    bool Exists { get; }

    IDirectory? GetParent();
    IDirectory[]? GetAncestry();
  }

  public interface IDirectory : IIoObject, IEquatable<IDirectory> {
    DirectoryInfo Info { get; }

    bool Create();

    IEnumerable<IDirectory> GetExistingSubdirs();
    IDirectory TryToGetSubdir(string relativePath, bool create = false);

    IEnumerable<IFile> GetExistingFiles();

    IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false);

    IFile TryToGetFile(string relativePath);
  }

  public interface IFile : IIoObject, IEquatable<IFile> {
    FileInfo Info { get; }

    string Extension { get; }
    IFile CloneWithExtension(string newExtension);

    string FullNameWithoutExtension { get; }
    string NameWithoutExtension { get; }

    StreamReader ReadAsText();
    byte[] SkimAllBytes();

    FileStream OpenRead();
    FileStream OpenWrite();
  }


  public interface ISerializable {
    public void Write(EndianBinaryWriter w);
  }

  public interface IDeserializable {
    public void Read(EndianBinaryReader r);
  }
}