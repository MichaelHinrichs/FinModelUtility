using System;
using System.IO;

using fin.image;
using fin.image.io;
using fin.util.color;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.io.image.tile {
  /// <summary>
  ///   Seems EERILY similar to the DXT1 format--they might actually be exactly
  ///   the same.
  /// </summary>
  public class CmprTileReader : ITileReader<Rgba32> {
    public IImage<Rgba32> CreateImage(int width, int height)
      => new Rgba32Image(PixelFormat.DXT1, width, height);

    private const int SUB_TILE_COUNT_IN_AXIS = 2;
    private const int SUB_TILE_SIZE_IN_AXIS = 4;

    private const int TILE_SIZE_IN_AXIS =
        CmprTileReader.SUB_TILE_COUNT_IN_AXIS *
        CmprTileReader.SUB_TILE_SIZE_IN_AXIS;

    public int TileWidth => CmprTileReader.TILE_SIZE_IN_AXIS;
    public int TileHeight => CmprTileReader.TILE_SIZE_IN_AXIS;

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgba32* scan0,
                              int tileX,
                              int tileY,
                              int imageWidth,
                              int imageHeight) {
      for (var j = 0; j < SUB_TILE_COUNT_IN_AXIS; ++j) {
        for (var i = 0; i < SUB_TILE_COUNT_IN_AXIS; ++i) {
          DecodeCmprSubblock_(
              er, 
              scan0,
              tileX * TileWidth + i * SUB_TILE_SIZE_IN_AXIS,
              tileY * TileHeight + j * SUB_TILE_SIZE_IN_AXIS,
              imageWidth);
        }
      }
    }

    private static unsafe void DecodeCmprSubblock_(
        IEndianBinaryReader er,
        Rgba32* scan0,
        int imageX,
        int imageY,
        int imageWidth) {
      Span<Rgba32> palette = stackalloc Rgba32[4];
      DecodeCmprPalette_(palette, er.ReadUInt16(), er.ReadUInt16());

      for (var j = 0; j < 4; ++j) {
        var indices = er.ReadByte();
        for (var i = 0; i < 4; ++i) {
          var index = (indices >> (2 * (3 - i))) & 0b11;
          scan0[(imageY + j) * imageWidth + imageX + i] = palette[index];
        }
      }
    }

    private static void DecodeCmprPalette_(Span<Rgba32> palette,
                                           ushort color1Value,
                                           ushort color2Value) {
      ColorUtil.SplitRgb565(color1Value, out var r1, out var g1, out var b1);
      ColorUtil.SplitRgb565(color2Value, out var r2, out var g2, out var b2);

      palette[0] = new Rgba32(r1, g1, b1, 255);
      palette[1] = new Rgba32(r2, g2, b2, 255);

      if (color1Value > color2Value) {
        // 3rd color in palette is 1/3 from 1st to 2nd.
        palette[2] = new Rgba32(
            (byte) ((((int) r1 << 1) + (int) r2) / 3),
            (byte) ((((int) g1 << 1) + (int) g2) / 3),
            (byte) ((((int) b1 << 1) + (int) b2) / 3),
            byte.MaxValue);
        // 4th color in palette is 2/3 from 1st to 2nd.
        palette[3] = new Rgba32(
            (byte) (((int) r1 + ((int) r2 << 1)) / 3),
            (byte) (((int) g1 + ((int) g2 << 1)) / 3),
            (byte) (((int) b1 + ((int) b2 << 1)) / 3),
            byte.MaxValue);
      } else {
        // 3rd color in palette is halfway between 1st and 2nd.
        palette[2] = new Rgba32(
            (byte) (((int) r1 + (int) r2) >> 1),
            (byte) (((int) g1 + (int) g2) >> 1),
            (byte) (((int) b1 + (int) b2) >> 1),
            byte.MaxValue);
        // 4th color in palette is transparency, automatically 0.
      }
    }
  }
}