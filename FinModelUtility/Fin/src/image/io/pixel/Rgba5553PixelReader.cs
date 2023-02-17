using System.IO;

using SixLabors.ImageSharp.PixelFormats;


namespace fin.image.io {
  public class Rgba5553PixelReader : IPixelReader<Rgba32> {
    public IImage<Rgba32> CreateImage(int width, int height)
      => new Rgba32Image(width, height);

    public unsafe void Decode(IEndianBinaryReader er,
                              Rgba32* scan0,
                              int offset) {
      var firstByte = er.ReadByte();
      var secondByte = er.ReadByte();

      if (((int) firstByte & 128) == 0) {
        scan0[offset] = new Rgba32(
            (byte) ((int) secondByte << 4 & 240 | (int) secondByte & 15),
            (byte) ((int) secondByte & 240 | (int) secondByte >> 4 & 15),
            (byte) ((int) firstByte << 4 & 240 | (int) firstByte & 15),
            (byte) ((int) firstByte << 1 & 224 | (int) firstByte >> 2 & 28 |
                    (int) firstByte >> 5 & 3));
      } else {
        scan0[offset] = new Rgba32(
            (byte) ((int) secondByte << 3 & 248 | (int) secondByte >> 2 & 7),
            (byte) ((int) firstByte << 6 & 192 | (int) secondByte >> 2 & 56 |
                    (int) firstByte & 6 | (int) secondByte >> 7 & 1),
            (byte) ((int) firstByte << 1 & 248 | (int) firstByte >> 4 & 7),
            255);
      }
    }
  }
}