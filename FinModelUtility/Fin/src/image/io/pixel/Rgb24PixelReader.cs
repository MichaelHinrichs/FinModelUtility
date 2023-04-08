using System.IO;

using fin.color;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Helper class for reading 24-bit RGB pixels.
  /// </summary>
  public class Rgb24PixelReader : IPixelReader<Rgb24> {
    public IImage<Rgb24> CreateImage(int width, int height)
      => new Rgb24Image(PixelFormat.RGB888, width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgb24* scan0,
                              int offset) {
      FinColor.SplitRgb(er.ReadInt24(), out var r, out var g, out var b);
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}