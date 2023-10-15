using fin.io;
using fin.io.bundles;

namespace fin.archives {
  public readonly struct ArchiveExtractionOptions {
    public ArchiveExtractionOptions() { }

    public bool DeleteArchiveFile { get; init; } = true;
  }

  public interface IArchiveExtractor<in TArchiveFileBundle>
      where TArchiveFileBundle : IArchiveFileBundle {
    IReadOnlyTreeDirectory OpenArchive(TArchiveFileBundle bundle);

    bool ExtractArchive(
        IAnnotatedFileBundle<TArchiveFileBundle> annotatedBundle,
        out ISystemDirectory directory,
        ArchiveExtractionOptions options = default);
  }
}