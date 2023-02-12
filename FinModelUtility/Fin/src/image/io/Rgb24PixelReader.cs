using System.IO;

using fin.color;

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
      FinColor.SplitRgb(er.ReadInt24(), out var r, out var g, out var b);
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}