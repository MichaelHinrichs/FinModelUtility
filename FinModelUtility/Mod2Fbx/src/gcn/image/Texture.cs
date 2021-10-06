using System.Collections.Generic;
using System.Drawing;
using System.IO;

using fin.util.asserts;

using mod.gcn.image;

namespace mod.gcn {
  public class Texture : IGcnSerializable {
    public int index;

    public ushort width = 0;
    public ushort height = 0;
    public TextureFormat format = 0;
    public uint unknown = 0;
    public readonly List<byte> imageData = new();

    public enum TextureFormat {
      RGB565 = 0,
      CMPR = 1,
      RGB5A3 = 2,
      I4 = 3,
      I8 = 4,
      IA4 = 5,
      IA8 = 6,
      RGBA32 = 8,
    }

    public string Name => "texture" + this.index + "_" + this.format;

    public void Read(EndianBinaryReader reader) {
      this.width = reader.ReadUInt16();
      this.height = reader.ReadUInt16();
      this.format = (TextureFormat) reader.ReadUInt32();
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
      writer.Write((uint) this.format);
      writer.Write(this.unknown);

      for (var i = 0; i < 4; i++) {
        writer.Write((uint) 0);
      }

      writer.Write(this.imageData.Count);
      foreach (var b in this.imageData) {
        writer.Write(b);
      }
    }

    public Bitmap ToBitmap() {
      BImageFormat? imageFormat = null;
      if (this.format == TextureFormat.RGB5A3) {
        imageFormat = new Rgb5A3(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.RGB565) {
        imageFormat = new Rgb565(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.CMPR) {
        imageFormat = new Cmpr(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.I4) {
        imageFormat = new I4(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.I8) {
        imageFormat = new I8(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.IA4) {
        imageFormat = new IA4(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.IA8) {
        imageFormat = new IA8(this.imageData, this.width, this.height);
      } else {
        Asserts.Fail($"Unsupported type: {this.format}");
      }

      return imageFormat.Image;
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