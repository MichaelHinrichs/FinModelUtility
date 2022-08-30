using System;
using System.Drawing;
using System.IO;


namespace fin.image {
  public enum LocalImageFormat {
    BMP,
    PNG,
    JPEG,
    GIF,
  }


  public interface IImage : IDisposable {
    int Width { get; }
    int Height { get; }

    delegate void Rgba32GetHandler(int x,
                                          int y,
                                          out byte r,
                                          out byte g,
                                          out byte b,
                                          out byte a);

    delegate void AccessHandler(Rgba32GetHandler getHandler);

    void Access(AccessHandler accessHandler);

    bool HasAlphaChannel { get; }

    Bitmap AsBitmap();

    void ExportToStream(Stream stream, LocalImageFormat imageFormat);
  }
}