using System;
using System.Runtime.CompilerServices;

using fin.image;
using fin.image.io;
using fin.util.color;

using schema.binary;

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
      Span<Rgba32> paletteBuffer = stackalloc Rgba32[4];
      Span<byte> indicesBuffer = stackalloc byte[4];

      for (var j = 0; j < SUB_TILE_COUNT_IN_AXIS; ++j) {
        for (var i = 0; i < SUB_TILE_COUNT_IN_AXIS; ++i) {
          DecodeCmprSubblock_(
              er,
              scan0,
              paletteBuffer,
              indicesBuffer,
              tileX * TileWidth + i * SUB_TILE_SIZE_IN_AXIS,
              tileY * TileHeight + j * SUB_TILE_SIZE_IN_AXIS,
              imageWidth);
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void DecodeCmprSubblock_(
        IEndianBinaryReader er,
        Rgba32* scan0,
        Span<Rgba32> paletteBuffer,
        Span<byte> indicesBuffer,
        int imageX,
        int imageY,
        int imageWidth) {
      DecodeCmprPalette_(paletteBuffer, er.ReadUInt16(), er.ReadUInt16());

      er.ReadBytes(indicesBuffer);
      for (var j = 0; j < 4; ++j) {
        var indices = indicesBuffer[j];
        for (var i = 0; i < 4; ++i) {
          var index = (indices >> (2 * (3 - i))) & 0b11;
          scan0[(imageY + j) * imageWidth + imageX + i] = paletteBuffer[index];
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DecodeCmprPalette_(Span<Rgba32> palette,
                                           ushort color1Value,
                                           ushort color2Value) {
      ColorUtil.SplitRgb565(color1Value, out var r1, out var g1, out var b1);
      ColorUtil.SplitRgb565(color2Value, out var r2, out var g2, out var b2);

      palette[0] = new Rgba32(r1, g1, b1);
      palette[1] = new Rgba32(r2, g2, b2);

      if (color1Value > color2Value) {
        // 3rd color in palette is 1/3 from 1st to 2nd.
        palette[2] = new Rgba32(
            (byte) (((r1 << 1) + r2) / 3),
            (byte) (((g1 << 1) + g2) / 3),
            (byte) (((b1 << 1) + b2) / 3));
        // 4th color in palette is 2/3 from 1st to 2nd.
        palette[3] = new Rgba32(
            (byte) ((r1 + (r2 << 1)) / 3),
            (byte) ((g1 + (g2 << 1)) / 3),
            (byte) ((b1 + (b2 << 1)) / 3));
      } else {
        // 3rd color in palette is halfway between 1st and 2nd.
        palette[2] = new Rgba32(
            (byte) ((r1 + r2) >> 1),
            (byte) ((g1 + g2) >> 1),
            (byte) ((b1 + b2) >> 1));
        // 4th color in palette is transparency.
        palette[3] = default;
      }
    }
  }
}