using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class L8PixelReader : IPixelReader<L8> {
    public IImage<L8> CreateImage_(int width, int height)
      => new I8Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                                          L8* scan0,
                                          int offset)
      => scan0[offset] = new L8(er.ReadByte());
  }
}