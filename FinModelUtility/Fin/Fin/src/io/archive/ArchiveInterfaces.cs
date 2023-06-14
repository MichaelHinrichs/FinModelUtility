using System.Collections.Generic;
using System.IO;

namespace fin.io.archive {
  public interface IArchiveContentFile {
    string RelativeName { get; }
  }

  public interface IArchiveStream<in TArchiveContentFile>
      where TArchiveContentFile : IArchiveContentFile {
    IEndianBinaryReader AsEndianBinaryReader();
    IEndianBinaryReader AsEndianBinaryReader(Endianness endianness);

    Stream GetContentFileStream(TArchiveContentFile archiveContentFile);

    void CopyContentFileInto(TArchiveContentFile archiveContentFile,
                             Stream dstStream);
  }

  public interface IArchiveReader<TArchiveContentFile>
      where TArchiveContentFile : IArchiveContentFile {
    bool IsValidArchive(Stream archive);

    IArchiveStream<TArchiveContentFile> Decompress(Stream archive);

    IEnumerable<TArchiveContentFile> GetFiles(
        IArchiveStream<TArchiveContentFile> archiveStream);
  }

  public interface IArchiveExtractor<TArchiveContentFile>
      where TArchiveContentFile : IArchiveContentFile {
    bool TryToExtractIntoNewDirectory<TArchiveReader>(
        Stream archive,
        ISystemDirectory newDirectory,
        out IFileHierarchy outFileHierarchy)
        where TArchiveReader : IArchiveReader<TArchiveContentFile>, new();
  }
}