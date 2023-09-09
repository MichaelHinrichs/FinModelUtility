using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  public class Gar5FileType : IGarFileType {
    public uint FileCount { get; }
    public int FirstFileIndex { get; }
    public int TypeNameOffset { get; }
    public string TypeName { get; }

    public IGarSubfile[] Files { get; }

    public Gar5FileType(
        IEndianBinaryReader er,
        GarHeader header,
        int fileTypeIndex) {
      er.Position = header.FileTypesOffset + 8 * 4 * fileTypeIndex;

      this.FileCount = er.ReadUInt32();
      er.ReadUInt32();
      this.FirstFileIndex = er.ReadInt32();
      this.TypeNameOffset = er.ReadInt32();
      er.ReadInt32();
      er.ReadUInt32();
      er.ReadUInt32();
      er.ReadUInt32();

      er.Position = this.TypeNameOffset;
      this.TypeName = er.ReadStringNT();

      this.Files = new IGarSubfile[this.FileCount];
      for (var i = 0; i < this.FileCount; ++i) {
        this.Files[i] = new Gar5Subfile(er, header, this, i);
      }
    }
  }
}