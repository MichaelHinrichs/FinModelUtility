namespace fin.image.io {
  /// <summary>
  ///   Basic indexer for tiles, where it assumes the tile pixels are laid out
  ///   row-wise.
  /// </summary>
  public class BasicTilePixelIndexer : ITilePixelIndexer {
    private readonly int tileWidth_;

    public BasicTilePixelIndexer(int tileWidth) {
      this.tileWidth_ = tileWidth;
    }

    public void GetPixelInTile(int index, out int x, out int y) {
      x = index % this.tileWidth_;
      y = index / this.tileWidth_;
    }
  }
}