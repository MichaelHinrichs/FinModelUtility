using fin.io;
using fin.io.archive;
using fin.util.asserts;
using fin.util.strings;

using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  public class Gar2FileType : IGarFileType {
    public int FileCount { get; }
    public int FileListOffset { get; }
    public int TypeNameOffset { get; }
    public string TypeName { get; }

    public IGarSubfile[] Files { get; }

    public Gar2FileType(
        IEndianBinaryReader er,
        GarHeader header,
        int fileTypeIndex) {
      er.Position = header.FileTypesOffset + 16 * fileTypeIndex;

      this.FileCount = er.ReadInt32();
      this.FileListOffset = er.ReadInt32();
      this.TypeNameOffset = er.ReadInt32();
      er.ReadInt32();

      er.Position = this.TypeNameOffset;
      this.TypeName = er.ReadStringNT();

      this.Files = new IGarSubfile[Math.Max(0, this.FileCount)];
      for (var i = 0; i < this.FileCount; ++i) {
        this.Files[i] = new Gar2Subfile(er, header, this, i);
      }
    }
  }
}