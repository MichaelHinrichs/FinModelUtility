namespace fin.io {
  public readonly struct ArchiveFile {
    public required string LocalPath { get; init; }
    public required int Offset { get; init; }
    public required int Length { get; init; }
  }

  public class Archive {
    /*public Archive(IList<ArchiveFile> archiveFiles) {
      var sortedFiles =
          archiveFiles.OrderBy(archiveFile => archiveFile.LocalPath).ToArray();

      foreach (var archiveFile in sortedFiles) { }
    }

    private readonly struct ArchiveDirectory : IReadOnlyTreeDirectory { }

    private readonly struct ArchiveFile : IReadOnlyTreeFile { }*/
  }
}