using System;
using System.IO;

using fin.image;
using fin.image.io;

namespace f3dzex2.image {
  public enum N64ImageFormat : byte {
    // Note: "1 bit per pixel" is not a Fast3D format.
    _1BPP = 0x00,
    ARGB1555 = 0x10,
    RGBA8888 = 0x18,
    CI4 = 0x40,
    CI8 = 0x48,
    LA4 = 0x60,
    LA8 = 0x68,
    LA16 = 0x70,
    I4i = 0x80,
    I4ii = 0x90,
    I8 = 0x88,
  }

  public class N64ImageParser {
    public IImage Parse(N64ImageFormat format,
                        byte[] data,
                        int width,
                        int height,
                        ushort[] palette,
                        bool isPaletteRGBA16) {
      switch (format) {
        case N64ImageFormat.ARGB1555:
          return PixelImageReader.New(width,
                                      height,
                                      new Argb1555PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
        case N64ImageFormat.LA8:
          return PixelImageReader.New(width,
                                      height,
                                      new La8PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
        case N64ImageFormat.LA16:
          return PixelImageReader.New(width,
                                      height,
                                      new La16PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
        default:
          throw new NotImplementedException();
      }
    }
  }
}
