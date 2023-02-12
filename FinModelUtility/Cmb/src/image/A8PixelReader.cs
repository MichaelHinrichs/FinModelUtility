using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class A8PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage_(int width, int height)
      => new Ia16Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              La16* scan0,
                              int offset)
      => scan0[offset] = new La16(0xFF, er.ReadByte());
  }
}