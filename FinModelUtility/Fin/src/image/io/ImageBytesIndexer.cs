using System;


namespace fin.image.io {
  public class ImageBytesIndexer : IImageBytesIndexer {
    public ImageBytesIndexer(int imageWidth,
                             int imageHeight,
                             int bitsPerPixel) {
      this.ImageWidth = imageWidth;
      this.ImageHeight = imageHeight;
      this.BitsPerPixel = bitsPerPixel;
    }

    public int ImageWidth { get; }
    public int ImageHeight { get; }

    public int BitsPerPixel { get; }

    public void GetByteIndexOfPixel(
        int x,
        int y,
        out int index,
        out SubIndexType subIndexType) {
      var bitsUpToPixel = BitsPerPixel * (y * this.ImageWidth + x);
      if (this.BitsPerPixel < 8) {
        var floatIndex = bitsUpToPixel / 8f;
        index = (int)Math.Floor(floatIndex);
        subIndexType = Math.Abs(floatIndex - index) < .001
                           ? SubIndexType.UPPER
                           : SubIndexType.LOWER;
      } else {
        index = bitsUpToPixel / 8;
        subIndexType = SubIndexType.AT_INDEX;
      }
    }
  }
}