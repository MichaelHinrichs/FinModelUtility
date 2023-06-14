using System;
using System.Collections.Generic;

namespace fin.io.archive {
  public readonly struct ArchiveFile {
    public required string RelativeName { get; init; }
    public required Func<ReadOnlyMemory<byte>> GetDataHandler { get; init; }
  }

  public interface IArchiveReader {
    bool TryToGetFiles(IReadOnlyGenericFile file,
                       out IEnumerable<ArchiveFile> archiveFiles);
  }
}