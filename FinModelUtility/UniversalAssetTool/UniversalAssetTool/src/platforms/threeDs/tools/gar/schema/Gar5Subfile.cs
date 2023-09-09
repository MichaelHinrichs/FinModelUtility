using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  public class Gar5Subfile : IGarSubfile {
    public string FileName { get; }
    public string? FullPath { get; }

    public int Position { get; }
    public int Length { get; }

    public Gar5Subfile(
        IEndianBinaryReader er,
        GarHeader header,
        Gar5FileType fileType,
        int fileInFileTypeIndex) {
      er.Position = header.FileMetadataOffset +
                    (fileType.FirstFileIndex + fileInFileTypeIndex) * 4 * 4;
      var fileSize = er.ReadInt32();
      var fileOffset = er.ReadUInt32();
      var fileNameOffset = er.ReadInt32();
      var fullPathOffset = er.ReadInt32();


      er.Position = fileNameOffset;
      this.FileName = er.ReadStringNT();

      if (fullPathOffset != -1) {
        er.Position = fullPathOffset;
        this.FullPath = er.ReadStringNT();
      }

      if (Path.GetExtension(this.FileName) == string.Empty) {
        this.FileName += $".{fileType.TypeName}";

        if (this.FullPath != null) {
          this.FullPath += $".{fileType.TypeName}";
        }
      }

      this.Position = (int) fileOffset;
      this.Length = fileSize;
    }
  }
}