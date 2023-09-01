using System;

using fin.image;
using fin.image.formats;
using fin.schema;
using fin.util.color;

using gx;

using j3d.image;

using schema.binary;
using schema.binary.attributes;

using SixLabors.ImageSharp.PixelFormats;

namespace j3d.schema.bmd.tex1 {
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

  [BinarySchema]
  [LocalPositions]
  public partial class TextureEntry : IBinaryConvertible {
    public TextureFormat Format;
    public byte AlphaSetting;
    public ushort Width;
    public ushort Height;
    public GX_WRAP_TAG WrapS;
    public GX_WRAP_TAG WrapT;

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool PalettesEnabled;

    public PaletteFormat PaletteFormat;

    [WLengthOfSequence(nameof(palette))]
    public ushort NrPaletteEntries;

    public uint PaletteOffset;
    public uint BorderColor;
    public GX_MIN_TEXTURE_FILTER MinFilter;
    public GX_MAG_TEXTURE_FILTER MagFilter;
    [Unknown]
    public ushort Unknown4;
    public byte NrMipMap;
    [Unknown]
    public byte Unknown5;
    public ushort LodBias;

    [WPointerTo(nameof(Data))]
    public uint DataOffset;

    [RAtPosition(nameof(DataOffset))]
    [RSequenceLengthSource(nameof(CompressedBufferSize_))]
    public byte[] Data;

    [Ignore]
    public Rgba32[] palette;

    [ReadLogic]
    private void ReadPalettes_(IEndianBinaryReader er) {
      long position = er.Position;
      this.palette = new Rgba32[this.NrPaletteEntries];

      er.Position = this.PaletteOffset;
      for (var i = 0; i < this.NrPaletteEntries; ++i) {
        switch (this.PaletteFormat) {
          case PaletteFormat.PAL_A8_I8: {
            var alpha = er.ReadByte();
            var intensity = er.ReadByte();
            this.palette[i] =
                new Rgba32(intensity, intensity, intensity, alpha);
            break;
          }
          case PaletteFormat.PAL_R5_G6_B5: {
            ColorUtil.SplitRgb565(er.ReadUInt16(),
                                  out var r,
                                  out var b,
                                  out var g);
            this.palette[i] = new Rgba32(r, g, b);
            break;
          }
          // TODO: There seems to be a bug reading the palette, these colors look weird
          case PaletteFormat.PAL_A3_RGB5: {
            ColorUtil.SplitRgb5A3(er.ReadUInt16(),
                                  out var r,
                                  out var g,
                                  out var b,
                                  out var a);
            this.palette[i] = new Rgba32(r, g, b, a);
            break;
          }
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      er.Position = position;
    }

    public unsafe IImage ToBitmap() {
      try {
        return new J3dImageReader(this.Width, this.Height, this.Format).ReadImage(
            this.Data,
            Endianness.BigEndian);
      } catch { }

      var width = this.Width;
      var height = this.Height;

      Rgba32Image bitmap;
      var isIndex4 = this.Format == TextureFormat.INDEX4;
      var isIndex8 = this.Format == TextureFormat.INDEX8;
      if (isIndex4 || isIndex8) {
        bitmap = new Rgba32Image(isIndex4 ? PixelFormat.P4 : PixelFormat.P8,
                                 width,
                                 height);
        using var imageLock = bitmap.Lock();
        var ptr = imageLock.pixelScan0;

        var indices = new byte[width * height];
        if (isIndex4) {
          for (var i = 0; i < this.Data.Length; ++i) {
            var two = this.Data[i];

            var firstIndex = two >> 4;
            var secondIndex = two & 0x0F;

            indices[2 * i + 0] = (byte) firstIndex;
            indices[2 * i + 1] = (byte) secondIndex;
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
                ptr[(ty * blockHeight + y) * width + (tx * blockWidth + x)] =
                    this.palette[indices[index++]];
              }
            }
          }
        }

        return bitmap;
      }

      throw new NotImplementedException();
    }

    [Ignore]
    private int CompressedBufferSize_ {
      get {
        int num1 = (int) this.Width + (8 - (int) this.Width % 8) % 8;
        int num2 = (int) this.Width + (4 - (int) this.Width % 4) % 4;
        int num3 = (int) this.Height + (8 - (int) this.Height % 8) % 8;
        int num4 = (int) this.Height + (4 - (int) this.Height % 4) % 4;
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
            _                        => -1
        };
      }
    }
  }
}