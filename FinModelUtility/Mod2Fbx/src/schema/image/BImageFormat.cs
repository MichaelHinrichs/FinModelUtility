using System.Collections.Generic;

using fin.image;
using fin.model;


namespace mod.schema.image {
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

      this.Image = new Rgba32Image(imageWidth, imageHeight);

      this.LoadImage_(rawData);
    }

    public Rgba32Image Image { get; }

    protected abstract IColor[] DecodeBlock(IList<byte> block, int position);

    private void LoadImage_(IList<byte> rawData) {
      this.Image.Mutate((_, setHandler) => {
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

                setHandler(x,
                           y,
                           finColor.Rb,
                           finColor.Gb,
                           finColor.Bb,
                           finColor.Ab);
              }
            }
          }
        }
      });
    }
  }
}