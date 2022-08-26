using fin.data;
using fin.schema.color;
using fin.util.array;

using schema;


namespace modl.schema.terrain {
  public partial class HeightmapParser : IBwHeightmap {
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
              var schemaTile = tilesEr.ReadNew<SchemaTile>();

              for (var pointY = 0; pointY < 4; ++pointY) {
                for (var pointX = 0; pointX < 4; ++pointX) {
                  var point = new BwHeightmapPoint();
                  tile.Points[pointX, pointY] = point;

                  var xyScale = 64;
                  point.X = xyScale * (16 * chunkX + 4 * tileX + pointX);
                  point.Y = xyScale * (16 * chunkY + 4 * tileY + pointY);

                  var pointOffset = pointY * 4 + pointX;

                  point.Height = schemaTile.Heights[pointOffset];
                  point.LightColor = schemaTile.LightColors[pointOffset];
                }
              }
            }
          }
        }
      }
    }

    [BinarySchema]
    private partial class SchemaTile : IBiSerializable {
      public ushort[] Heights { get; } = new ushort[16];

      public Rgba32[] LightColors { get; } =
        Arrays.From(16, () => new Rgba32());

      public byte[] Unknown { get; } = new byte[84];
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
      public Rgba32 LightColor { get; set; }
    }
  }
}