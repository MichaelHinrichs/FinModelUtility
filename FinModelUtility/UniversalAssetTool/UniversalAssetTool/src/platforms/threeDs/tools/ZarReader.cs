using fin.io;
using fin.io.archive;
using fin.util.strings;

using schema.binary;
using schema.binary.attributes;

namespace uni.platforms.threeDs.tools {
  public partial class ZarReader : IArchiveReader<SubArchiveContentFile> {
    public bool IsValidArchive(Stream archive)
      => MagicTextUtil.Verify(archive, "ZAR" + AsciiUtil.GetChar(1));

    public IArchiveStream<SubArchiveContentFile> Decompress(Stream archive)
      => new SubArchiveStream(archive);

    public IEnumerable<SubArchiveContentFile> GetFiles(
        IArchiveStream<SubArchiveContentFile> archiveStream) {
      var er = archiveStream.AsEndianBinaryReader(Endianness.LittleEndian);
      var zar = new Zar(er);

      foreach (var fileType in zar.FileTypes) {
        foreach (var file in fileType.Files) {
          yield return new SubArchiveContentFile {
              RelativeName = file.FileName,
              Position = file.Position,
              Length = file.Length,
          };
        }
      }
    }

    private class Zar {
      public ZarHeader Header { get; }
      public ZarFileType[] FileTypes { get; }

      public Zar(IEndianBinaryReader er) {
        this.Header = er.ReadNew<ZarHeader>();

        this.FileTypes = new ZarFileType[this.Header.FileTypeCount];
        for (var i = 0; i < this.FileTypes.Length; ++i) {
          this.FileTypes[i] = new ZarFileType(er, this.Header, i);
        }
      }
    }

    [BinarySchema]
    private partial class ZarHeader : IBinaryConvertible {
      private readonly string magic_ = "ZAR" + AsciiUtil.GetChar(1);

      public int Size { get; set; }

      public short FileTypeCount { get; set; }
      public short FileCount { get; set; }

      public int FileTypesOffset { get; set; }
      public int FileMetadataOffset { get; set; }
      public int DataOffset { get; set; }
    }

    private class ZarFileType {
      public int FileCount { get; }
      public int FileListOffset { get; }
      public int TypeNameOffset { get; }
      public string TypeName { get; }

      public ZarSubfile[] Files { get; }

      public ZarFileType(
          IEndianBinaryReader er,
          ZarHeader header,
          int fileTypeIndex) {
        er.Position = header.FileTypesOffset + 16 * fileTypeIndex;

        this.FileCount = er.ReadInt32();
        this.FileListOffset = er.ReadInt32();
        this.TypeNameOffset = er.ReadInt32();
        er.ReadInt32();

        /*er.Position = this.TypeNameOffset;
        this.TypeName = er.ReadStringNT(Encoding.UTF8);*/

        this.Files = new ZarSubfile[this.FileCount];
        for (var i = 0; i < this.FileCount; ++i) {
          this.Files[i] = new ZarSubfile(er, header, this, i);
        }
      }
    }

    private class ZarSubfile {
      public string FileName { get; }

      public int Position { get; }
      public int Length { get; }

      public ZarSubfile(
          IEndianBinaryReader er,
          ZarHeader header,
          ZarFileType fileType,
          int fileInFileTypeIndex) {
        er.Position = fileType.FileListOffset + 4 * fileInFileTypeIndex;
        var fileIndex = er.ReadInt32();

        er.Position = header.FileMetadataOffset + 8 * fileIndex;
        var fileSize = er.ReadInt32();
        var fileNameOffset = er.ReadInt32();

        er.Position = fileNameOffset;
        this.FileName = er.ReadStringNT(StringEncodingType.UTF8);

        er.Position = header.DataOffset + 4 * fileIndex;
        var fileOffset = er.ReadInt32();

        this.Position = fileOffset;
        this.Length = fileSize;
      }
    }
  }
}