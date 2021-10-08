using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using fin.model;
using fin.util.image;

namespace mod.gcn.image {
  public abstract class BImageFormat {
    private readonly int imageWidth_;
    private readonly int imageHeight_;
    private readonly int blockWidth_;
    private readonly int blockHeight_;
    private readonly int blockSize_;

    public BImageFormat(
        IList<byte> rawData,
        int imageWidth,
        int imageHeight,
        int blockWidth,
        int blockHeight,
        int bitsPerPixel) {
      this.imageWidth_ = imageWidth;
      this.imageHeight_ = imageHeight;
      this.blockWidth_ = blockWidth;
      this.blockHeight_ = blockHeight;
      this.blockSize_ = (int) (blockWidth * blockHeight * (bitsPerPixel / 8f));

      this.Image =
          new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);

      this.LoadImage_(rawData);
    }

    public Bitmap Image { get; }

    protected abstract IColor[] DecodeBlock(IList<byte> block, int position);

    private void LoadImage_(IList<byte> rawData)
      => BitmapUtil.InvokeAsLocked(this.Image, rawData, this.LoadImageUtil_);

    private unsafe void LoadImageUtil_(
        BitmapData bmpData,
        IList<byte> rawData) {
      var pos = 0;

      var bytes = (byte*) bmpData.Scan0.ToPointer();
      for (var j = 0; j < this.imageHeight_ / this.blockHeight_; ++j) {
        for (var i = 0; i < this.imageWidth_ / this.blockWidth_; ++i) {
          var decodedBlock = this.DecodeBlock(rawData, pos);
          pos += this.blockSize_;

          for (var r = 0; r < this.blockHeight_; ++r) {
            for (var c = 0; c < this.blockWidth_; ++c) {
              var x = i * this.blockWidth_ + c;
              var y = j * this.blockHeight_ + r;

              var finColor = decodedBlock[r * this.blockWidth_ + c];

              var index = y * this.imageWidth_ + x;
              bytes[4 * index + 0] = finColor.Bb;
              bytes[4 * index + 1] = finColor.Gb;
              bytes[4 * index + 2] = finColor.Rb;
              bytes[4 * index + 3] = finColor.Ab;
            }
          }
        }
      }
    }
  }
}