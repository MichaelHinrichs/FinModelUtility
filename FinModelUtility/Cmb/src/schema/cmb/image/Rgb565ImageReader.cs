using System.IO;

using fin.image;
using fin.util.color;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.schema.cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class Rgb565ImageReader : BTiledImageReader<Rgb24> {
    public Rgb565ImageReader(int width, int height) : base(width, height) { }

    protected override IImage<Rgb24> CreateImage_(int width, int height)
      => new Rgb24Image(width, height);

    protected override unsafe void Decode(IEndianBinaryReader er,
                                          Rgb24* scan0,
                                          int offset) {
      var value = er.ReadUInt16();
      ColorUtil.SplitRgb565(value, out var r, out var g, out var b);
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}