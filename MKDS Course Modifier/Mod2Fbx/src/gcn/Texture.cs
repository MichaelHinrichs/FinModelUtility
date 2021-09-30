using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  public class Texture : IGcnSerializable {
    public ushort width = 0;
    public ushort height = 0;
    public uint format = 0;
    public uint unknown = 0;
    public readonly List<byte> imageData = new();

    public void Read(EndianBinaryReader reader) {
      this.width = reader.ReadUInt16();
      this.height = reader.ReadUInt16();
      this.format = reader.ReadUInt32();
      this.unknown = reader.ReadUInt32();

      for (var i = 0; i < 4; i++) {
        reader.ReadUInt32();
      }

      var numImageData = reader.ReadUInt32();
      this.imageData.Clear();
      for (var i = 0; i < numImageData; ++i) {
        this.imageData.Add(reader.ReadByte());
      }
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.width);
      writer.Write(this.height);
      writer.Write(this.format);
      writer.Write(this.unknown);
      
      for (var i = 0; i < 4; i++) {
        writer.Write((uint) 0);
      }

      writer.Write(this.imageData.Count);
      foreach (var b in this.imageData) {
        writer.Write(b);
      }
    }
  }

  public class TextureAttributes : IGcnSerializable {
    public ushort index = 0;
    public ushort tilingMode = 0;
    public ushort unknown1 = 0;
    public float unknown2 = 0;

    public void Read(EndianBinaryReader reader) {
      this.index = reader.ReadUInt16();
      reader.ReadUInt16();
      this.tilingMode = reader.ReadUInt16();
      this.unknown1 = reader.ReadUInt16();
      this.unknown2 = reader.ReadSingle();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.index);
      writer.Write((ushort) 0);
      writer.Write(this.tilingMode);
      writer.Write(this.unknown1);
      writer.Write(this.unknown2);
    }
  }
}