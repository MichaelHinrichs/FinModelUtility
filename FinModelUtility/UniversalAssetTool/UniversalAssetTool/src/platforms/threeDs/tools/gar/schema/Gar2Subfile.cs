using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  public class Gar2Subfile : IGarSubfile {
    public string FileName { get; }
    public string FullPath { get; }

    public int Position { get; }
    public int Length { get; }

    public Gar2Subfile(
        IEndianBinaryReader er,
        GarHeader header,
        Gar2FileType fileType,
        int fileInFileTypeIndex) {
      er.Position = fileType.FileListOffset + 4 * fileInFileTypeIndex;
      var fileIndex = er.ReadInt32();

      er.Position = header.FileMetadataOffset + 12 * fileIndex;
      var fileSize = er.ReadInt32();
      var fileNameOffset = er.ReadInt32();
      var fullPathOffset = er.ReadInt32();

      er.Position = fileNameOffset;
      this.FileName = er.ReadStringNT();

      er.Position = fullPathOffset;
      this.FullPath = er.ReadStringNT();

      er.Position = header.DataOffset + 4 * fileIndex;
      var fileOffset = er.ReadInt32();

      this.Position = fileOffset;
      this.Length = Math.Max(fileSize, 0);
    }
  }
}