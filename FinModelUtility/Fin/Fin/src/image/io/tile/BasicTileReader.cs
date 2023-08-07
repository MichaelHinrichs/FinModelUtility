using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io.tile {
  public class BasicTileReader<TPixel>
      : ITileReader<TPixel> where TPixel : unmanaged, IPixel<TPixel> {
    private readonly IPixelIndexer pixelIndexer_;
    private readonly IPixelReader<TPixel> pixelReader_;

    public BasicTileReader(
        int tileWidth,
        int tileHeight,
        IPixelIndexer pixelIndexer,
        IPixelReader<TPixel> pixelReader) {
      this.TileWidth = tileWidth;
      this.TileHeight = tileHeight;
      this.pixelIndexer_ = pixelIndexer;
      this.pixelReader_ = pixelReader;
    }

    public IImage<TPixel> CreateImage(int width, int height)
      => this.pixelReader_.CreateImage(width, height);

    public int TileWidth { get; }
    public int TileHeight { get; }

    public unsafe void Decode(IEndianBinaryReader er,
                              TPixel* scan0,
                              int tileX,
                              int tileY,
                              int imageWidth,
                              int imageHeight) {
      var xx = tileX * this.TileWidth;
      var yy = tileY * this.TileHeight;

      for (var i = 0;
           i < this.TileWidth * this.TileHeight;
           i += this.pixelReader_.PixelsPerRead) {
        this.pixelIndexer_.GetPixelCoordinates(i, out var x, out var y);
        var dstOffs = (yy + y) * imageWidth + xx + x;
        this.pixelReader_.Decode(er, scan0, dstOffs);
      }
    }
  }
}