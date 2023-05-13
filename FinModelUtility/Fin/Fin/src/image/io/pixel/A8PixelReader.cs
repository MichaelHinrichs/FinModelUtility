using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  /// <summary>
  ///   Helper class for reading 8-bit alpha pixels.
  /// </summary>
  public class A8PixelReader : IPixelReader<La16> {
    public IImage<La16> CreateImage(int width, int height)
      => new La16Image(PixelFormat.A8, width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              La16* scan0,
                              int offset)
      => scan0[offset] = new La16(0xFF, er.ReadByte());
  }
}