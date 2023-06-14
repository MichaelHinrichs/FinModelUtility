using System;
using System.Collections.Generic;
using System.IO;

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
    public bool TryToExtractIntoNewDirectory<TArchiveReader>(
        Stream archive,
        ISystemDirectory systemDirectory,
        out IFileHierarchy outFileHierarchy)
        where TArchiveReader : IArchiveReader<SubArchiveContentFile>, new() {
      if (systemDirectory.Exists) {
        outFileHierarchy = new FileHierarchy(systemDirectory);
        return true;
      }

      var archiveReader = new TArchiveReader();
      if (!archiveReader.IsValidArchive(archive)) {
        outFileHierarchy = default;
        return false;
      }

      var archiveStream = archiveReader.Decompress(archive);

      var archiveContentFiles = archiveReader.GetFiles(archiveStream);
      systemDirectory.Create();

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

      outFileHierarchy = new FileHierarchy(systemDirectory);
      return true;
    }
  }
}