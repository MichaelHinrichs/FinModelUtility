using System;
using System.Collections.Generic;
using System.Drawing;
using fin.math;
using fin.model;

namespace mod.gcn.image {
  public abstract class BImageFormat {
    private readonly int imageWidth_;
    private readonly int imageHeight_;
    private readonly int blockWidth_;
    private readonly int blockHeight_;
    private readonly int blockSize_;
    private readonly int bitsPerPixel_;

    public BImageFormat(
        IList<byte> rawData,
        int imageWidth,
        int imageHeight,
        int blockWidth,
        int blockHeight,
        int blockSize,
        int bitsPerPixel) {
      this.imageWidth_ = imageWidth;
      this.imageHeight_ = imageHeight;
      this.blockWidth_ = blockWidth;
      this.blockHeight_ = blockHeight;
      this.blockSize_ = blockSize;
      this.bitsPerPixel_ = bitsPerPixel;
      this.Image = new Bitmap(imageWidth, imageHeight);

      this.LoadImage_(rawData);
    }

    public Bitmap Image { get; }

    protected abstract IColor[] DecodeBlock(IList<byte> block, int position);

    private void LoadImage_(IList<byte> rawData) {
      var pos = 0;
      for (var j = 0; j < this.imageHeight_ / this.blockHeight_; ++j) {
        for (var i = 0; i < this.imageWidth_ / this.blockWidth_; ++i) {
          var decodedBlock = this.DecodeBlock(rawData, pos);
          pos += this.blockSize_;

          for (var r = 0; r < this.blockHeight_; ++r) {
            for (var c = 0; c < this.blockWidth_; ++c) {
              var x = i * this.blockWidth_ + c;
              var y = j * this.blockHeight_ + r;

              var finColor = decodedBlock[r * this.blockWidth_ + c];
              var color = Color.FromArgb(finColor.Ab,
                                         finColor.Rb,
                                         finColor.Gb,
                                         finColor.Bb);
              this.Image.SetPixel(x, y, color);
            }
          }
        }
      }
    }

    protected byte ExtractScaled(ushort col, int offset, int count) {
      var maxPossible = Math.Pow(2, count);
      var factor = 255 / maxPossible;
      return this.ExtractScaled(col, offset, count, factor);
    }

    protected byte ExtractScaled(ushort col, int offset, int count, double factor) {
      var extracted = BitLogic.ExtractFromRight(col, offset, count) * 1d;
      return (byte)Math.Round(extracted * factor);
    }

  }
}