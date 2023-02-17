using System.IO;

using fin.util.color;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Helper class for reading 16-bit RGBA pixels, where each channel has 4
  ///   bits.
  /// </summary>
  public class Rgba4444PixelReader : IPixelReader<Rgba32> {
    public IImage<Rgba32> CreateImage(int width, int height)
      => new Rgba32Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                                          Rgba32* scan0,
                                          int offset) {
      var value = er.ReadUInt16();
      ColorUtil.SplitRgba4444(value,
                              out var r,
                              out var g,
                              out var b,
                              out var a);
      scan0[offset] = new Rgba32(r, g, b, a);
    }
  }
}