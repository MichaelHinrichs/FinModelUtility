using fin.data;
using fin.schema.color;
using fin.util.array;

using schema;


namespace modl.schema.terrain {
  public partial class HeightmapParser : IBwHeightmap {
    // TODO: Write this in a more schema way instead

    public Grid<IBwHeightmapChunk?> Chunks { get; } = new(64, 64);

    public HeightmapParser(byte[] tilemapBytes,
                           byte[] tilesBytes) {
      using var tilemapEr =
          new EndianBinaryReader(new MemoryStream(tilemapBytes));
      using var tilesEr =
          new EndianBinaryReader(new MemoryStream(tilesBytes));

      var maxOffset = -1;

      tilemapEr.ReadNewArray<SchemaTilemapDefinition>(
          out var tilemapDefinitions, 64 * 64);
      for (var chunkY = 0; chunkY < 64; ++chunkY) {
        for (var chunkX = 0; chunkX < 64; ++chunkX) {
          var tilemapDefinition = tilemapDefinitions[chunkY * 64 + chunkX];
          if (tilemapDefinition.Unknown != 1) {
            continue;
          }

          var offset = tilemapDefinition.Offset;
          maxOffset = Math.Max(maxOffset, offset);

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

      // TODO: There's 15 more blocks of 180 bytes for each tile, what does it mean???
    }

    [BinarySchema]
    private partial class SchemaTilemapDefinition : IBiSerializable {
      private readonly byte padding_ = 0;

      public byte Unknown { get; private set; }

      public ushort Offset { get; private set; }
    }

    [BinarySchema]
    private partial class SchemaTile : IBiSerializable {
      public ushort[] Heights { get; } = new ushort[16];

      public Rgba32[] LightColors { get; } =
        Arrays.From(16, () => new Rgba32());

      public byte[] Unknown { get; } = new byte[76];

      public uint MaybeMaterial { get; private set; }

      public uint Unknown2 { get; private set; }
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