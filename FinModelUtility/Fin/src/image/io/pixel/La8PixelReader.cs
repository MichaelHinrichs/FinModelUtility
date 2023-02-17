using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Helper class for reading 8-bit luminance/alpha pixels.
  /// </summary>
  public class La8PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage(int width, int height)
      => new Ia16Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              La16* scan0,
                              int offset) {
      var value = er.ReadByte();

      var upper = (byte) ((value >> 4) * 17);
      var lower = (byte) ((value & 0xF) * 17);

      byte luminance;
      byte alpha;
      if (er.IsOppositeEndiannessOfSystem) {
        luminance = lower;
        alpha = upper;
      } else {
        luminance = upper;
        alpha = lower;
      }

      scan0[offset] = new La16(luminance, alpha);
    }
  }
}