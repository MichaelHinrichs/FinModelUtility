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

    Bitmap AsBitmap();

    void ExportToStream(Stream stream, LocalImageFormat imageFormat);
  }
}