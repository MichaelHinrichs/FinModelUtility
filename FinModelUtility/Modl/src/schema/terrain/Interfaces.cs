using fin.data;
using fin.schema.color;


namespace modl.schema.terrain {
  public interface IBwHeightmap {
    Grid<IBwHeightmapChunk?> Chunks { get; } 
  }

  public interface IBwHeightmapChunk {
    Grid<IBwHeightmapTile> Tiles { get; }
  }

  public interface IBwHeightmapTile {
    Grid<IBwHeightmapPoint> Points { get; }
  }

  public interface IBwHeightmapPoint {
    int X { get; }
    int Y { get; }
    ushort Height { get; }

    Rgba32 LightColor { get; }
  }
}