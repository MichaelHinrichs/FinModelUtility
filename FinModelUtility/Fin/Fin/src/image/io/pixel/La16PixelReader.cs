using fin.image.formats;

using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io.pixel {
  /// <summary>
  ///   Helper class for reading 16-bit luminance/alpha pixels.
  /// </summary>
  public class La16PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage(int width, int height)
      => new La16Image(PixelFormat.LA88, width, height);

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