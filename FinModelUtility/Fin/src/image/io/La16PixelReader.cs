using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class La16PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage_(int width, int height)
      => new Ia16Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              La16* scan0,
                              int offset) {
      var la = er.ReadUInt16();
      var l = (byte) (la >> 8);
      var a = (byte) (la & 0xFF);
      scan0[offset] = new La16(l, a);
    }
  }
}