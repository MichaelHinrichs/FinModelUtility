using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class La8PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage(int width, int height)
      => new Ia16Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              La16* scan0,
                              int offset) {
      var value = er.ReadByte();

      var luminance = (byte) ((value >> 4) * 17);
      var alpha = (byte) ((value & 0xF) * 17);

      scan0[offset] = new La16(luminance, alpha);
    }
  }
}