using System.IO;

using fin.image;
using fin.image.io;
using fin.io.image.tile;

using TextureFormat = mod.schema.Texture.TextureFormat;


namespace mod.image {
  public class ModImageReader : IImageReader {
    private readonly IImageReader impl_;

    public ModImageReader(int width, int height, TextureFormat format) {
      this.impl_ = this.CreateImpl_(width, height, format);
    }

    private IImageReader CreateImpl_(int width,
                                     int height,
                                     TextureFormat format) {
      return format switch {
          TextureFormat.I4 => TiledImageReader.New(
              width,
              height,
              8,
              8,
              new L4PixelReader(),
              Endianness.BigEndian),
          TextureFormat.I8 => TiledImageReader.New(
              width,
              height,
              8,
              4,
              new L8PixelReader(),
              Endianness.BigEndian),
          TextureFormat.IA4 => TiledImageReader.New(
              width,
              height,
              8,
              4,
              new La8PixelReader(),
              Endianness.BigEndian),
          TextureFormat.IA8 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new La16PixelReader(),
              Endianness.BigEndian),
          TextureFormat.RGB565 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgb565PixelReader(),
              Endianness.BigEndian),
          TextureFormat.RGB5A3 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgba5553PixelReader(),
              Endianness.BigEndian),
          TextureFormat.RGBA32 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgba32PixelReader(),
              Endianness.BigEndian),
          TextureFormat.CMPR => TiledImageReader.New(
              width,
              height,
              new CmprTileReader(),
              Endianness.BigEndian),
      };
    }

    public IImage Read(byte[] srcBytes) => this.impl_.Read(srcBytes);
  }
}