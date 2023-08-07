using System;
using System.Collections.Generic;
using System.IO;

using schema.binary;

using SubstreamSharp;

namespace fin.io.archive {
  public readonly struct SubArchiveContentFile : IArchiveContentFile {
    public required string RelativeName { get; init; }
    public required int Position { get; init; }
    public required int Length { get; init; }
  }

  public class SubArchiveStream : IArchiveStream<SubArchiveContentFile> {
    private readonly Stream impl_;

    public SubArchiveStream(Stream impl) {
      this.impl_ = impl;
    }

    public IEndianBinaryReader AsEndianBinaryReader()
      => new EndianBinaryReader(this.impl_);

    public IEndianBinaryReader AsEndianBinaryReader(Endianness endianness)
      => new EndianBinaryReader(this.impl_, endianness);

    public Stream GetContentFileStream(
        SubArchiveContentFile archiveContentFile)
      => this.impl_.Substream(archiveContentFile.Position,
                              archiveContentFile.Length);

    public void CopyContentFileInto(SubArchiveContentFile archiveContentFile,
                                    Stream dstStream) {
      this.impl_.Position = archiveContentFile.Position;

      Span<byte> buffer = stackalloc byte[81920];
      for (var i = 0; i < archiveContentFile.Length; i += buffer.Length) {
        var remaining = archiveContentFile.Length - i;
        var target = remaining > buffer.Length
            ? buffer
            : buffer.Slice(0, remaining);

        this.impl_.Read(target);
        dstStream.Write(target);
      }
    }
  }

  public class SubArchiveExtractor : IArchiveExtractor<SubArchiveContentFile> {
    public ArchiveExtractionResult TryToExtractIntoNewDirectory<TArchiveReader>(
        IReadOnlyGenericFile archive,
        ISystemDirectory systemDirectory)
        where TArchiveReader : IArchiveReader<SubArchiveContentFile>, new() {
      if (systemDirectory.Exists) {
        return ArchiveExtractionResult.ALREADY_EXISTS;
      }

      systemDirectory.Create();

      using var fs = archive.OpenRead();
      return this.TryToExtractIntoExistingDirectory_<TArchiveReader>(
          fs,
          systemDirectory);
    }

    public ArchiveExtractionResult TryToExtractIntoNewDirectory<TArchiveReader>(
        Stream archive,
        ISystemDirectory systemDirectory)
        where TArchiveReader : IArchiveReader<SubArchiveContentFile>, new() {
      if (systemDirectory.Exists) {
        return ArchiveExtractionResult.ALREADY_EXISTS;
      }

      systemDirectory.Create();

      return this.TryToExtractIntoExistingDirectory_<TArchiveReader>(
          archive,
          systemDirectory);
    }

    private ArchiveExtractionResult TryToExtractIntoExistingDirectory_<
        TArchiveReader>(
        Stream archive,
        ISystemDirectory systemDirectory)
        where TArchiveReader : IArchiveReader<SubArchiveContentFile>, new() {
      var archiveReader = new TArchiveReader();
      if (!archiveReader.IsValidArchive(archive)) {
        return ArchiveExtractionResult.FAILED;
      }

      var archiveStream = archiveReader.Decompress(archive);

      var archiveContentFiles = archiveReader.GetFiles(archiveStream);

      var createdDirectories = new HashSet<string>();
      foreach (var archiveContentFile in archiveContentFiles) {
        var dstFile = new FinFile(Path.Join(systemDirectory.FullName,
                                            archiveContentFile.RelativeName));

        var dstDirectory = dstFile.GetParentFullName()!;
        if (createdDirectories.Add(dstDirectory)) {
          FinFileSystem.Directory.CreateDirectory(dstDirectory);
        }

        using var dstFileStream = dstFile.OpenWrite();
        archiveStream.CopyContentFileInto(archiveContentFile, dstFileStream);
      }

      return ArchiveExtractionResult.NEWLY_EXTRACTED;
    }
  }
}