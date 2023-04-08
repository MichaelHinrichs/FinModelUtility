using System.IO;

using fin.color;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Helper class for reading 32-bit RGBA pixels.
  /// </summary>
  public class Rgba32PixelReader : IPixelReader<Rgba32> {
    public IImage<Rgba32> CreateImage(int width, int height)
      => new Rgba32Image(PixelFormat.RGBA8888, width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgba32* scan0,
                              int offset) {
      FinColor.SplitRgba(er.ReadInt32(), out var r, out var g, out var b, out var a);
      scan0[offset] = new Rgba32(r, g, b, a);
    }
  }
}