using dat.image;

using fin.image;
using fin.image.formats;
using fin.util.color;

using gx;

using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

namespace dat.schema {
  /// <summary>
  ///   Texture object.
  ///
  ///   Shamelessly copied from:
  ///    - https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1281
  ///    - https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/LibWii/TLP.cs#L166
  ///    - https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRaw/Common/HSD_TOBJ.cs#L92
  /// </summary>
  public class TObj : IBinaryDeserializable {
    public uint StringOffset { get; private set; }
    public string? Name { get; set; }

    public IImage Image { get; private set; }

    public uint NextTObjOffset { get; private set; }
    public TObj? NextTObj { get; private set; }

    public unsafe void Read(IBinaryReader br) {
      this.StringOffset = br.ReadUInt32();
      this.NextTObjOffset = br.ReadUInt32();

      br.Position += 4 * 11;

      var wrapS = br.ReadUInt32();
      var wrapT = br.ReadUInt32();
      var scaleW = br.ReadByte();
      var scaleH = br.ReadByte();

      br.Position += 2 + 12;

      var imageOffset = br.ReadUInt32();
      var paletteOffset = br.ReadUInt32();

      br.Position = imageOffset;
      var imageDataOffset = br.ReadUInt32();
      var width = br.ReadUInt16();
      var height = br.ReadUInt16();
      var format = (GxTextureFormat) br.ReadUInt32();

      br.Position = paletteOffset;
      var paletteDataOffset = br.ReadUInt32();
      var paletteFormat = (GxPaletteFormat) br.ReadUInt32();
      var unk1 = br.ReadUInt32();
      var paletteEntryCount = br.ReadUInt16();
      var unk2 = br.ReadUInt16();

      // TODO: Add support for indexed textures
      try {
        br.Position = imageDataOffset;
        this.Image = new DatImageReader(width, height, format).ReadImage(br);
      } catch {
        var isIndex4 = format == GxTextureFormat.INDEX4;
        var isIndex8 = format == GxTextureFormat.INDEX8;
        if (isIndex4 || isIndex8) {
          var palette = new Rgba32[paletteEntryCount];
          br.Position = paletteDataOffset;
          for (var i = 0; i < paletteEntryCount; ++i) {
            switch (paletteFormat) {
              case GxPaletteFormat.PAL_A8_I8: {
                var alpha = br.ReadByte();
                var intensity = br.ReadByte();
                palette[i] =
                    new Rgba32(intensity, intensity, intensity, alpha);
                break;
              }
              case GxPaletteFormat.PAL_R5_G6_B5: {
                // Curiously, the colors are flipped here.
                ColorUtil.SplitRgb565(br.ReadUInt16(),
                                      out var r,
                                      out var g,
                                      out var b);
                palette[i] = new Rgba32(r, g, b);
                break;
              }
              // TODO: There seems to be a bug reading the palette, these colors look weird
              case GxPaletteFormat.PAL_A3_RGB5: {
                ColorUtil.SplitRgb5A3(br.ReadUInt16(),
                                      out var r,
                                      out var g,
                                      out var b,
                                      out var a);
                palette[i] = new Rgba32(r, g, b, a);
                break;
              }
              default:
                throw new ArgumentOutOfRangeException();
            }
          }


          var bitmap = new Rgba32Image(isIndex4 ? PixelFormat.P4 : PixelFormat.P8,
                                   width,
                                   height);
          this.Image = bitmap;

          using var imageLock = bitmap.Lock();
          var ptr = imageLock.pixelScan0;

          br.Position = imageDataOffset;
          var dataLength = width * height;
          if (isIndex4) {
            dataLength >>= 1;
          }

          var data = br.ReadBytes(dataLength);
          byte[] indices;
          if (isIndex4) {
            indices = new byte[width * height];
            for (var i = 0; i < data.Length; ++i) {
              var two = data[i];

              var firstIndex = two >> 4;
              var secondIndex = two & 0x0F;

              indices[2 * i + 0] = (byte) firstIndex;
              indices[2 * i + 1] = (byte) secondIndex;
            }
          } else {
            indices = data;
          }

          var blockWidth = 8;
          var blockHeight = isIndex4 ? 8 : 4;

          var index = 0;
          for (var ty = 0; ty < height / blockHeight; ty++) {
            for (var tx = 0; tx < width / blockWidth; tx++) {
              for (var y = 0; y < blockHeight; ++y) {
                for (var x = 0; x < blockWidth; ++x) {
                  ptr[(ty * blockHeight + y) * width + (tx * blockWidth + x)] =
                      palette[indices[index++]];
                }
              }
            }
          }
        }
      }

      if (this.NextTObjOffset != 0) {
        br.Position = this.NextTObjOffset;
        this.NextTObj = br.ReadNew<TObj>();
      }
    }
  }
}