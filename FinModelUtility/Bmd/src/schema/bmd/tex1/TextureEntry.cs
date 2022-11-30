using Chadsoft.CTools.Image;
using fin.color;
using fin.image;
using fin.util.color;
using gx;
using schema;
using System;
using System.ComponentModel;
using System.IO;


namespace bmd.schema.bmd.tex1 {
  public enum TextureFormat : byte {
    I4 = 0,
    I8 = 1,
    A4_I4 = 2,
    A8_I8 = 3,
    R5_G6_B5 = 4,
    A3_RGB5 = 5,
    ARGB8 = 6,
    INDEX4 = 8,
    INDEX8 = 9,
    INDEX14_X2 = 10, // 0x0000000A
    S3TC1 = 14, // 0x0000000E
  }

  public enum PaletteFormat : byte {
    PAL_A8_I8,
    PAL_R5_G6_B5,
    PAL_A3_RGB5,
  }


  public class TextureEntry : IDeserializable {
    private readonly long baseOffset_;

    // Do not modify any of these types or the order!
    public TextureFormat Format;
    public Byte AlphaSetting;
    public UInt16 Width;
    public UInt16 Height;
    public GX_WRAP_TAG WrapS;
    public GX_WRAP_TAG WrapT;
    public Byte PalettesEnabled;
    public PaletteFormat PaletteFormat;
    public UInt16 NrPaletteEntries;
    public UInt32 PaletteOffset;
    public IColor[] palette;
    public UInt32 BorderColor;
    public GX_TEXTURE_FILTER MinFilter;
    public GX_TEXTURE_FILTER MagFilter;
    public UInt16 Unknown4;
    public Byte NrMipMap;
    public Byte Unknown5;
    public UInt16 LodBias;
    public UInt32 DataOffset;

    public byte[] Data;

    public TextureEntry(long baseoffset = 0) {
      this.baseOffset_ = baseoffset;
    }

    public void Read(EndianBinaryReader er) {
      var pos = er.Position;

      this.Format = (TextureFormat)er.ReadByte();
      this.AlphaSetting = er.ReadByte();
      this.Width = er.ReadUInt16();
      this.Height = er.ReadUInt16();
      this.WrapS = (GX_WRAP_TAG)er.ReadByte();
      this.WrapT = (GX_WRAP_TAG)er.ReadByte();
      this.PalettesEnabled = er.ReadByte();
      this.PaletteFormat = (PaletteFormat)er.ReadByte();
      this.NrPaletteEntries = er.ReadUInt16();
      this.PaletteOffset = er.ReadUInt32();
      this.BorderColor = er.ReadUInt32();
      this.MinFilter = (GX_TEXTURE_FILTER)er.ReadByte();
      this.MagFilter = (GX_TEXTURE_FILTER)er.ReadByte();
      this.Unknown4 = er.ReadUInt16();
      this.NrMipMap = er.ReadByte();
      this.Unknown5 = er.ReadByte();
      this.LodBias = er.ReadUInt16();
      this.DataOffset = er.ReadUInt32();

      long position = er.Position;
      {
        er.Position = this.baseOffset_ + this.DataOffset;
        this.Data = er.ReadBytes(this.GetCompressedBufferSize());
      }

      this.palette = new IColor[this.NrPaletteEntries];
      {
        er.Position = pos + this.PaletteOffset;
        for (var i = 0; i < this.NrPaletteEntries; ++i) {
          switch (this.PaletteFormat) {
            case PaletteFormat.PAL_A8_I8: {
              var alpha = er.ReadByte();
              var intensity = er.ReadByte();
              this.palette[i] =
                  FinColor.FromRgbaBytes(intensity,
                                         intensity,
                                         intensity,
                                         alpha);
              break;
            }
            case PaletteFormat.PAL_R5_G6_B5: {
              this.palette[i] = ColorUtil.ParseRgb565(er.ReadUInt16());
              break;
            }
            // TODO: There seems to be a bug reading the palette, these colors look weird
            case PaletteFormat.PAL_A3_RGB5: {
              this.palette[i] = ColorUtil.ParseRgb5A3(er.ReadUInt16());
              break;
            }
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
      er.Position = position;
    }

    // TODO: Share this implementation w/ BTI
    public unsafe IImage ToBitmap() {
      var width = this.Width;
      var height = this.Height;

      Rgba32Image bitmap;
      var isIndex4 = this.Format == TextureFormat.INDEX4;
      var isIndex8 = this.Format == TextureFormat.INDEX8;
      if (isIndex4 || isIndex8) {
        bitmap = new Rgba32Image(width, height);
        bitmap.Mutate((_, setHandler) => {
          var indices = new byte[width * height];
          if (isIndex4) {
            for (var i = 0; i < this.Data.Length; ++i) {
              var two = this.Data[i];

              var firstIndex = two >> 4;
              var secondIndex = two & 0x0F;

              indices[2 * i + 0] = (byte)firstIndex;
              indices[2 * i + 1] = (byte)secondIndex;
            }
          } else {
            indices = this.Data;
          }

          var blockWidth = 8;
          var blockHeight = isIndex4 ? 8 : 4;

          var index = 0;
          for (var ty = 0; ty < height / blockHeight; ty++) {
            for (var tx = 0; tx < width / blockWidth; tx++) {
              for (var y = 0; y < blockHeight; ++y) {
                for (var x = 0; x < blockWidth; ++x) {
                  var color = this.palette[indices[index++]];
                  setHandler(
                      tx * blockWidth + x,
                      ty * blockHeight + y,
                      color.Rb, color.Gb, color.Bb, color.Ab);
                }
              }
            }
          }
        });

        return bitmap;
      }

      ImageDataFormat imageDataFormat = this.Format switch {
          TextureFormat.I4       => ImageDataFormat.I4,
          TextureFormat.I8       => ImageDataFormat.I8,
          TextureFormat.A4_I4    => ImageDataFormat.IA4,
          TextureFormat.A8_I8    => ImageDataFormat.IA8,
          TextureFormat.R5_G6_B5 => ImageDataFormat.RGB565,
          TextureFormat.A3_RGB5  => ImageDataFormat.RGB5A3,
          TextureFormat.ARGB8    => ImageDataFormat.Rgba32,
          TextureFormat.S3TC1    => ImageDataFormat.Cmpr,
          _                      => throw new NotImplementedException()
      };

      byte[] numArray = imageDataFormat.ConvertFrom(
          this.Data, width, height, (ProgressChangedEventHandler)null);
      bitmap = new Rgba32Image(width, height);
      bitmap.Mutate((_, setHandler) => {
        for (var y = 0; y < height; ++y) {
          for (var x = 0; x < width; ++x) {
            var i = 4 * (y * width + x);

            var b = numArray[i + 0];
            var g = numArray[i + 1];
            var r = numArray[i + 2];
            var a = numArray[i + 3];

            setHandler(x, y, r, g, b, a);
          }
        }
      });
      return bitmap;
    }

    private int GetCompressedBufferSize() {
      int num1 = (int)this.Width + (8 - (int)this.Width % 8) % 8;
      int num2 = (int)this.Width + (4 - (int)this.Width % 4) % 4;
      int num3 = (int)this.Height + (8 - (int)this.Height % 8) % 8;
      int num4 = (int)this.Height + (4 - (int)this.Height % 4) % 4;
      return this.Format switch {
          TextureFormat.I4         => num1 * num3 / 2,
          TextureFormat.I8         => num1 * num4,
          TextureFormat.A4_I4      => num1 * num4,
          TextureFormat.A8_I8      => num2 * num4 * 2,
          TextureFormat.R5_G6_B5   => num2 * num4 * 2,
          TextureFormat.A3_RGB5    => num2 * num4 * 2,
          TextureFormat.ARGB8      => num2 * num4 * 4,
          TextureFormat.INDEX4     => num1 * num3 / 2,
          TextureFormat.INDEX8     => num1 * num4,
          TextureFormat.INDEX14_X2 => num2 * num4 * 2,
          TextureFormat.S3TC1      => num2 * num4 / 2,
          _                                        => -1
      };
    }
  }
}