using fin.data;
using fin.schema.color;


namespace modl.schema.terrain {
  public class HeightmapParser : IBwHeightmap {
    // TODO: Write this in a more schema way instead

    public Grid<IBwHeightmapChunk?> Chunks { get; } = new(64, 64);

    public HeightmapParser(byte[] tilemapBytes, byte[] tilesBytes) {
      using var tilemapEr =
          new EndianBinaryReader(new MemoryStream(tilemapBytes));
      using var tilesEr =
          new EndianBinaryReader(new MemoryStream(tilesBytes));

      for (var chunkY = 0; chunkY < 64; ++chunkY) {
        for (var chunkX = 0; chunkX < 64; ++chunkX) {
          var chunkOffset = 4 * (chunkY * 64 + chunkX);
          tilemapEr.Position = chunkOffset;
          var a = tilemapEr.ReadByte();
          var b = tilemapEr.ReadByte();
          var offset = tilemapEr.ReadUInt16();

          if (b != 1) {
            continue;
          }

          var chunk = new BwHeightmapChunk();
          this.Chunks[chunkX, chunkY] = chunk;

          for (var tileY = 0; tileY < 4; ++tileY) {
            for (var tileX = 0; tileX < 4; ++tileX) {
              var tile = new BwHeightmapTile();
              chunk.Tiles[tileX, tileY] = tile;

              var tileOffset = 4 * tileY + tileX;
              tilesEr.Position = 180 * (16 * offset + tileOffset);
              var tileBytes = tilesEr.ReadBytes(180);
              using var tileEr =
                  new EndianBinaryReader(new MemoryStream(tileBytes));

              for (var pointY = 0; pointY < 4; ++pointY) {
                for (var pointX = 0; pointX < 4; ++pointX) {
                  var point = new BwHeightmapPoint();
                  tile.Points[pointX, pointY] = point;

                  var xyScale = 64;
                  point.X = xyScale * (16 * chunkX + 4 * tileX + pointX);
                  point.Y = xyScale * (16 * chunkY + 4 * tileY + pointY);

                  var pointOffset = pointY * 4 + pointX;

                  var heightOffset = 2 * pointOffset;
                  tileEr.Position = heightOffset;
                  point.Height = tileEr.ReadUInt16();

                  var lightOffset = 32 + 4 * pointOffset;
                  tileEr.Position = lightOffset;
                  point.LightColor.Read(tileEr);
                }
              }
            }
          }
        }
      }
    }

    private class BwHeightmapChunk : IBwHeightmapChunk {
      public Grid<IBwHeightmapTile> Tiles { get; } = new(4, 4);
    }

    private class BwHeightmapTile : IBwHeightmapTile {
      public Grid<IBwHeightmapPoint> Points { get; } = new(4, 4);
    }

    private class BwHeightmapPoint : IBwHeightmapPoint {
      public int X { get; set; }
      public int Y { get; set; }
      public ushort Height { get; set; }
      public Rgba32 LightColor { get; } = new();
    }
  }
}