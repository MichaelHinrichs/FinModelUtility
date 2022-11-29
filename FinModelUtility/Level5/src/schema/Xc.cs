using fin.data;
using fin.io;
using level5.decompression;
using schema;

namespace level5.schema {
  public record XcFile(string Name, byte[] Data);

  public class Xc : IDeserializable {
    public ListDictionary<string, XcFile> FilesByExtension { get; } = new();

    public void Read(EndianBinaryReader er) {
      er.AssertString("XPCK");

      var fileCount = er.ReadUInt16() & 0xfff;

      var fileInfoOffset = er.ReadUInt16() * 4;
      var fileTableOffset = er.ReadUInt16() * 4;
      var dataOffset = er.ReadUInt16() * 4;

      er.ReadUInt16();
      var filenameTableSize = er.ReadUInt16() * 4;

      var hashToData = new Dictionary<uint, byte[]>();
      er.Position = fileInfoOffset;
      for (int i = 0; i < fileCount; i++) {
        var nameCrc = er.ReadUInt32();
        er.ReadInt16();
        var offset = (uint)er.ReadUInt16();
        var size = (uint)er.ReadUInt16();
        var offsetExt = (uint)er.ReadByte();
        var sizeExt = (uint)er.ReadByte();

        offset |= offsetExt << 16;
        size |= sizeExt << 16;
        offset = (uint)(offset * 4 + dataOffset);

        hashToData.Add(nameCrc, er.ReadBytesAtOffset(offset, (int) size));
      }

      byte[] nameTable = er.ReadBytesAtOffset(fileTableOffset, filenameTableSize);
      if (!Decompress.CheckLevel5Zlib(nameTable, out nameTable)) {
        nameTable = new LzssDecompressor().Decompress(nameTable);
      }

      this.FilesByExtension.Clear();
      using (var nt = new EndianBinaryReader(new MemoryStream(nameTable), er.Endianness)) {
        for (int i = 0; i < fileCount; i++) {
          var name = nt.ReadStringNT();

          var crc = Crc32.Crc32C(name);
          if (hashToData.ContainsKey(crc)) {
            this.FilesByExtension.Add(Path.GetExtension(name), new XcFile(name, hashToData[crc]));
          } else {
            Console.WriteLine("Couldn't find " + name + " " + crc.ToString("X"));
          }
        }
      }
    }
  }
}