using System.IO.Abstractions;

namespace fin.io {
  public interface IReadOnlyGenericFile {
    string DisplayFullPath { get; }
    FileSystemStream OpenRead();
  }

  public interface IGenericFile : IReadOnlyGenericFile {
    FileSystemStream OpenWrite();
  }
}