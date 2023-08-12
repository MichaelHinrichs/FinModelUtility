using fin.image;
using fin.image.io;
using fin.image.io.pixel;
using fin.image.io.tile;

using schema.binary;

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
              new L4PixelReader()),
          TextureFormat.I8 => TiledImageReader.New(
              width,
              height,
              8,
              4,
              new L8PixelReader()),
          TextureFormat.IA4 => TiledImageReader.New(
              width,
              height,
              8,
              4,
              new La8PixelReader()),
          TextureFormat.IA8 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new La16PixelReader()),
          TextureFormat.RGB565 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgb565PixelReader()),
          TextureFormat.RGB5A3 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgba5553PixelReader()),
          TextureFormat.RGBA32 => TiledImageReader.New(
              width,
              height,
              4,
              4,
              new Rgba32PixelReader()),
          TextureFormat.CMPR => TiledImageReader.New(
              width,
              height,
              new CmprTileReader()),
      };
    }

    public IImage Read(IEndianBinaryReader er) => this.impl_.Read(er);

    public IImage Read(byte[] data, Endianness endianness)
      => this.impl_.Read(data, endianness);
  }
}