using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.schema.cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class A8ImageReader : BTiledImageReader<La16> {
    public A8ImageReader(int width, int height) : base(width, height) { }

    protected override IImage<La16> CreateImage_(int width, int height)
      => new Ia16Image(width, height);

    protected override unsafe void Decode(IEndianBinaryReader er,
                                          La16* scan0,
                                          int offset)
      => scan0[offset] = new La16(0xFF, er.ReadByte());
  }
}