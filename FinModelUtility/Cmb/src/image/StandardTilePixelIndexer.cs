namespace cmb.image {
  public class StandardTilePixelIndexer : ITilePixelIndexer {
    private readonly int tileWidth_;

    public StandardTilePixelIndexer(int tileWidth) {
      this.tileWidth_ = tileWidth;
    }

    public void GetPixelInTile(int index, out int x, out int y) {
      x = index % this.tileWidth_;
      y = index / this.tileWidth_;
    }
  }
}