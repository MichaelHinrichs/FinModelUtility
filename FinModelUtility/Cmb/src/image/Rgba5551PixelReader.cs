using System.IO;

using fin.image;
using fin.util.color;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class Rgba5551PixelReader : IPixelReader<Rgba32> {
    public IImage<Rgba32> CreateImage_(int width, int height)
      => new Rgba32Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgba32* scan0,
                              int offset) {
      var value = er.ReadUInt16();
      ColorUtil.SplitRgb5A1(value, out var r, out var g, out var b, out var a);
      scan0[offset] = new Rgba32(r, g, b, a);
    }
  }
}