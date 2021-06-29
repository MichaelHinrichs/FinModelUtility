using System.Collections.Generic;
using System.IO;

namespace fin.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }

    IDirectory GetParent();
    bool Exists { get; }
  }

  public interface IDirectory : IIoObject {
    bool Create();

    IEnumerable<IDirectory> GetExistingSubdirs();
    IDirectory TryToGetSubdir(string relativePath, bool create = false);

    IEnumerable<IFile> GetExistingFiles();
    IEnumerable<IFile> SearchForFiles(string searchPattern);

    IFile TryToGetFile(string relativePath);
  }

  public interface IFile : IIoObject {
    string Extension { get; }
    IFile CloneWithExtension(string newExtension);
    
    StreamReader ReadAsText();
    byte[] SkimAllBytes();
  }
}