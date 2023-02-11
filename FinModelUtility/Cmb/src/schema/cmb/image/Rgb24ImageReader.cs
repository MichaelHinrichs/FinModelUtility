using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.schema.cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class Rgb24ImageReader : BTiledImageReader<Rgb24> {
    public Rgb24ImageReader(int width, int height) : base(width, height) { }

    protected override IImage<Rgb24> CreateImage_(int width, int height)
      => new Rgb24Image(width, height);

    protected override unsafe void Decode(IEndianBinaryReader er,
                                          Rgb24* scan0,
                                          int offset) {
      var b = er.ReadByte();
      var g = er.ReadByte();
      var r = er.ReadByte();
      scan0[offset] = new Rgb24(r, g, b);
    }
  }
}