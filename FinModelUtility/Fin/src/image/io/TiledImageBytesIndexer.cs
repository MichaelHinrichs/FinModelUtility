using System;


namespace fin.image.io {
  public class TiledImageBytesIndexer : ITiledImageBytesIndexer {
    public TiledImageBytesIndexer(int imageWidth,
                                  int imageHeight,
                                  int tileWidth,
                                  int tileHeight,
                                  int bitsPerPixel) {
      this.ImageWidth = imageWidth;
      this.ImageHeight = imageHeight;
      this.TileWidth = tileWidth;
      this.TileHeight = tileHeight;
      this.BitsPerPixel = bitsPerPixel;
    }

    public int ImageWidth { get; }
    public int ImageHeight { get; }

    public int TileWidth { get; }
    public int TileHeight { get; }

    public int BitsPerPixel { get; }

    public void GetByteIndexOfPixel(
        int x,
        int y,
        out int index,
        out SubIndexType subIndexType) {
      var tileX = x / this.TileWidth;
      var tileY = y / this.TileHeight;

      var bitsUpToCurrentTile =
          BitsPerPixel * (ImageWidth * TileHeight * tileY + TileWidth * tileX);

      var xInTile = x % this.TileWidth;
      var yInTile = y % this.TileHeight;

      var bitsInCurrentTile = BitsPerPixel * (yInTile * TileWidth + xInTile);

      var bitsUpToPixel = bitsUpToCurrentTile + bitsInCurrentTile;

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