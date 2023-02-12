using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class Rgb24PixelReader : IPixelReader<Rgb24> {
    public IImage<Rgb24> CreateImage_(int width, int height)
      => new Rgb24Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgb24* scan0,
                              int offset) {
      var b = er.ReadByte();
      var g = er.ReadByte();
      var r = er.ReadByte();
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}