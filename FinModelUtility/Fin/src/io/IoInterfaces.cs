using System;
using System.Collections.Generic;
using System.IO;


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

    IFile? TryToGetExistingFile(string relativePath);
    IFile GetExistingFile(string relativePath);
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
}