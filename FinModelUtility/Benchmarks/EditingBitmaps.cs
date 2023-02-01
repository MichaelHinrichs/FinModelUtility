using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace benchmarks {
  public unsafe class EditingBitmaps {
    private const int SIZE = 4000;

    private Bitmap bitmap_ =
        new(SIZE, SIZE, PixelFormat.Format32bppArgb);

    private BitmapData bmpData_;

    public static readonly Configuration ImageSharpConfig;

    static EditingBitmaps() {
      ImageSharpConfig = Configuration.Default.Clone();
      ImageSharpConfig.PreferContiguousImageBuffers = true;
    }

    private Image<Rgba32> image_ =
        new Image<Rgba32>(ImageSharpConfig, SIZE, SIZE);

    private MemoryHandle memoryHandle_;
    private Rgba32* imagePtr_;

    [IterationSetup]
    public void Setup() {
      this.bmpData_ = this.bitmap_.LockBits(
          new Rectangle(0, 0, SIZE, SIZE),
          ImageLockMode.ReadWrite,
          PixelFormat.Format32bppArgb);

      var frame = this.image_.Frames[0];
      frame.DangerousTryGetSinglePixelMemory(out var memory);

      this.memoryHandle_ = memory.Pin();
      this.imagePtr_ = (Rgba32*) this.memoryHandle_.Pointer;
    }

    [IterationCleanup]
    public void Cleanup() {
      this.bitmap_.UnlockBits(this.bmpData_);
      this.memoryHandle_.Dispose();
    }

    [Benchmark]
    public void ReadingBitmapBytes() {
      var ptr = (byte*) this.bmpData_.Scan0;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = 4 * (y * EditingBitmaps.SIZE + x);
          var b = ptr[i + 0];
          var g = ptr[i + 1];
          var r = ptr[i + 2];
          var a = ptr[i + 3];
        }
      }
    }

    [Benchmark]
    public void ReadingBitmapUints() {
      var ptr = (uint*) this.bmpData_.Scan0;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = y * EditingBitmaps.SIZE + x;
          var bgra = ptr[i];
          var b = bgra & 0xff;
          var g = (bgra >> 8) & 0xff;
          var r = (bgra >> 16) & 0xff;
          var a = bgra >> 24;
        }
      }
    }

    [Benchmark]
    public void ReadingBitmapUintsAndCasting() {
      var ptr = (uint*) this.bmpData_.Scan0;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = y * EditingBitmaps.SIZE + x;

          var bgra = ptr[i];
          var b = (byte) bgra;
          var g = (byte) (bgra >> 8);
          var r = (byte) (bgra >> 16);
          var a = (byte) (bgra >> 24);
        }
      }
    }

    [Benchmark]
    public void ReadingBitmapUintsSchenanigans() {
      var ptr = (uint*) this.bmpData_.Scan0;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var bgra = *(ptr++);
          var b = (byte) bgra;
          var g = (byte) (bgra >> 8);
          var r = (byte) (bgra >> 16);
          var a = (byte) (bgra >> 24);
        }
      }
    }

    [Benchmark]
    public void ReadingBitmapColors() {
      var ptr = (int*) this.bmpData_.Scan0;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = y * EditingBitmaps.SIZE + x;
          var color = Color.FromArgb(ptr[i]);
          var r = color.R;
          var g = color.G;
          var b = color.B;
          var a = color.A;
        }
      }
    }

    [Benchmark]
    public void ReadingImageBytes() {
      var ptr = (byte*) this.imagePtr_;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = 4 * (y * EditingBitmaps.SIZE + x);
          var r = ptr[i + 0];
          var g = ptr[i + 1];
          var b = ptr[i + 2];
          var a = ptr[i + 3];
        }
      }
    }

    [Benchmark]
    public void ReadingImageByteViaHandler() {
      var ptr = (byte*) this.imagePtr_;
      var handler = (int x,
                     int y,
                     out byte r,
                     out byte g,
                     out byte b,
                     out byte a)
          => {
        var value = ptr[y * EditingBitmaps.SIZE + x];
        r = (byte) (value & 0xff);
        g = (byte) ((value >> 8) & 0xff);
        b = (byte) ((value >> 16) & 0xff);
        a = (byte) (value >> 24);
      };

      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          handler(x, y, out var r, out var g, out var b, out var a);
        }
      }
    }

    [Benchmark]
    public void ReadingImageByteViaLambda() {
      var ptr = (byte*) this.imagePtr_;

      var handler = (int x,
                     int y,
                     byte r,
                     byte g,
                     byte b,
                     byte a)
          => { };

      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var value = ptr[y * EditingBitmaps.SIZE + x];

          var r = (byte) (value & 0xff);
          var g = (byte) ((value >> 8) & 0xff);
          var b = (byte) ((value >> 16) & 0xff);
          var a = (byte) (value >> 24);

          handler(x, y, r, g, b, a);
        }
      }
    }

    [Benchmark]
    public void ReadingImageUints() {
      var ptr = (uint*) this.imagePtr_;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = y * EditingBitmaps.SIZE + x;
          var bgra = ptr[i];
          var b = bgra & 0xff;
          var g = (bgra >> 8) & 0xff;
          var r = (bgra >> 16) & 0xff;
          var a = bgra >> 24;
        }
      }
    }

    [Benchmark]
    public void ReadingImageRgba32s() {
      var ptr = this.imagePtr_;
      for (var y = 0; y < SIZE; ++y) {
        for (var x = 0; x < SIZE; ++x) {
          var i = y * EditingBitmaps.SIZE + x;
          var rgba = ptr[i];
          var r = rgba.R;
          var g = rgba.G;
          var b = rgba.B;
          var a = rgba.A;
        }
      }
    }
  }
}