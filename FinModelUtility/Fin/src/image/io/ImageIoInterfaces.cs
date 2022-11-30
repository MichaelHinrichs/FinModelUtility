namespace fin.image.io {
  public enum SubIndexType {
    AT_INDEX,
    UPPER,
    LOWER,
  }

  public interface IImageBytesIndexer {
    int ImageWidth { get; }
    int ImageHeight { get; }

    int BitsPerPixel { get; }

    void GetByteIndexOfPixel(int x,
                             int y,
                             out int index,
                             out SubIndexType subIndexType);
  }

  public interface ITiledImageBytesIndexer : IImageBytesIndexer {
    int TileWidth { get; }
    int TileHeight { get; }
  }
}