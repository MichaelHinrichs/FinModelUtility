using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class L8ImageReader : BTiledImageReader<L8> {
    public L8ImageReader(int width, int height) : base(width, height) { }

    protected override IImage<L8> CreateImage_(int width, int height)
      => new I8Image(width, height);

    protected override unsafe void Decode(IEndianBinaryReader er,
                                          L8* scan0,
                                          int offset)
      => scan0[offset] = new L8(er.ReadByte());
  }
}