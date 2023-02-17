using System.IO;

using fin.util.color;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class Rgb565PixelReader : IPixelReader<Rgb24> {
    public IImage<Rgb24> CreateImage(int width, int height)
      => new Rgb24Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgb24* scan0,
                              int offset) {
      var value = er.ReadUInt16();
      ColorUtil.SplitRgb565(value, out var r, out var g, out var b);
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}