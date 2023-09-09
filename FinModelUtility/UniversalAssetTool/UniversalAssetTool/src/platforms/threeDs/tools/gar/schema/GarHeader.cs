using fin.util.asserts;

using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  public class GarHeader {
    public int Version { get; }

    public int Size { get; }

    public short FileTypeCount { get; }
    public short FileCount { get; }

    public int FileTypesOffset { get; }
    public int FileMetadataOffset { get; }
    public int DataOffset { get; }

    public GarHeader(IEndianBinaryReader er) {
      er.AssertString("GAR");

      this.Version = er.ReadByte();
      Asserts.True(this.Version is 2 or 5);

      this.Size = er.ReadInt32();

      this.FileTypeCount = er.ReadInt16();
      this.FileCount = er.ReadInt16();

      this.FileTypesOffset = er.ReadInt32();
      this.FileMetadataOffset = er.ReadInt32();
      this.DataOffset = er.ReadInt32();
    }
  }
}