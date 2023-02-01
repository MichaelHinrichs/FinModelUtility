using System;
using System.Buffers;
using System.Drawing;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fin.image {
  public enum LocalImageFormat {
    BMP,
    PNG,
    JPEG,
    GIF,
    TGA,
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

  public interface IImage<TPixel> : IImage
      where TPixel : unmanaged, IPixel<TPixel> {
    FinImageLock<TPixel> Lock();
  }

  /// <summary>
  ///   Based on how FastBitmap performs locking:
  ///   https://github.com/LuizZak/FastBitmap
  /// </summary>
  public unsafe struct FinImageLock<TPixel> : IDisposable
      where TPixel : unmanaged, IPixel<TPixel> {
    private bool isDisposed_ = false;
    private readonly MemoryHandle memoryHandle_;

    public FinImageLock(Image<TPixel> image) {
      var frame = image.Frames[0];
      frame.DangerousTryGetSinglePixelMemory(out var memory);

      this.memoryHandle_ = memory.Pin();
      this.byteScan0 = (byte*) this.memoryHandle_.Pointer;
      this.pixelScan0 = (TPixel*) this.byteScan0;
    }

    public void Dispose() {
      if (!this.isDisposed_) {
        this.isDisposed_ = true;
        this.memoryHandle_.Dispose();
      }
    }

    public readonly byte* byteScan0;
    public readonly TPixel* pixelScan0;
  }
}