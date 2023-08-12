using System;
using System.Drawing;
using System.IO;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace fin.image.formats {
  public abstract class BImage<TPixel> : IImage<TPixel>
      where TPixel : unmanaged, IPixel<TPixel> {
    protected BImage(PixelFormat format) {
      this.PixelFormat = format;
    }

    ~BImage() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.Impl.Dispose();

    protected abstract Image<TPixel> Impl { get; }

    public PixelFormat PixelFormat { get; }
    public int Width => this.Impl.Width;
    public int Height => this.Impl.Height;

    public abstract void Access(IImage.AccessHandler accessHandler);
    public abstract bool HasAlphaChannel { get; }

    public Bitmap AsBitmap() => FinImage.ConvertToBitmap(this);

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.Impl.Save(
          stream,
          FinImage.ConvertFinImageFormatToImageSharpEncoder(imageFormat));

    public FinImageLock<TPixel> Lock() => new(Impl);
  }
}