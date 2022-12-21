using fin.data;
using fin.schema.color;
using fin.util.array;
using schema;


namespace modl.schema.terrain {
  public partial class HeightmapParser : IBwHeightmap {
    // TODO: Write this in a more schema way instead

    private readonly TerrData terrData_;
    public const float overallScale = 5.25f;
    public const float xyScale = overallScale;
    public const float zScale = 1 / 64f * overallScale;

    public Grid<IBwHeightmapChunk?> Chunks { get; }

    public HeightmapParser(TerrData terrData,
                           byte[] tilemapBytes,
                           byte[] tilesBytes) {
      this.terrData_ = terrData;
      var chunkCountX = terrData.ChunkCountX;
      var chunkCountY = terrData.ChunkCountY;

      this.Chunks = new(chunkCountX, chunkCountY);

      SchemaTilemapDefinition[] tilemapDefinitions;
      {
        using var tilemapEr =
            new EndianBinaryReader(new MemoryStream(tilemapBytes),
                                   Endianness.BigEndian);
        tilemapEr.ReadNewArray(out tilemapDefinitions,
                               chunkCountX * chunkCountY);
      }

      SchemaTile[] schemaTiles;
      {
        using var tilesEr =
            new EndianBinaryReader(new MemoryStream(tilesBytes),
                                   Endianness.BigEndian);
        var schemaTileCount = tilesBytes.Length / 180;
        tilesEr.ReadNewArray(out schemaTiles, schemaTileCount);
      }

      var totalWidth = 12 * chunkCountX;
      var totalHeight = 12 * chunkCountY;

      for (var chunkY = 0; chunkY < chunkCountY; ++chunkY) {
        for (var chunkX = 0; chunkX < chunkCountX; ++chunkX) {
          var tilemapDefinition =
              tilemapDefinitions[chunkY * chunkCountX + chunkX];
          if (tilemapDefinition.Unknown != 1) {
            continue;
          }

          var offset = tilemapDefinition.Offset;

          var chunk = new BwHeightmapChunk();
          this.Chunks[chunkX, chunkY] = chunk;

          for (var tileY = 0; tileY < 4; ++tileY) {
            for (var tileX = 0; tileX < 4; ++tileX) {
              var tile = new BwHeightmapTile();
              chunk.Tiles[tileX, tileY] = tile;

              var tileOffset = 4 * tileY + tileX;
              var schemaTile = schemaTiles[16 * offset + tileOffset];

              tile.Schema = schemaTile;
              tile.MatlIndex = schemaTile.MatlIndex;

              for (var pointY = 0; pointY < 4; ++pointY) {
                for (var pointX = 0; pointX < 4; ++pointX) {
                  var point = new BwHeightmapPoint();
                  tile.Points[pointX, pointY] = point;

                  point.X = xyScale *
                            (4 * 3 * chunkX + 3 * tileX + pointX -
                             totalWidth / 2);
                  point.Y = xyScale * (4 * 3 * chunkY + 3 * tileY + pointY -
                                       totalHeight / 2);

                  var pointOffset = pointY * 4 + pointX;

                  point.Height = zScale * schemaTile.Heights[pointOffset];
                  point.LightColor = schemaTile.LightColors[pointOffset];
                }
              }
            }
          }
        }
      }
    }

    public float GetHeightAtPosition(float x, float y) {
      // TODO: Optimize this method
      var chunkCountX = this.terrData_.ChunkCountX;
      var chunkCountY = this.terrData_.ChunkCountY;

      for (var chunkY = 0; chunkY < chunkCountY; ++chunkY) {
        for (var chunkX = 0; chunkX < chunkCountX; ++chunkX) {
          var chunk = this.Chunks[chunkX, chunkY];
          if (chunk == null) {
            continue;
          }

          for (var tileY = 0; tileY < 4; ++tileY) {
            for (var tileX = 0; tileX < 4; ++tileX) {
              var tile = chunk.Tiles[tileX, tileY];
              if (tile == null) {
                continue;
              }

              for (var pointY = 0; pointY < 4; ++pointY) {
                for (var pointX = 0; pointX < 4; ++pointX) {
                  var point = tile.Points[pointX, pointY];

                  // TODO: Perform interpolation between the four corners
                  if (point.X >= x && point.Y >= y) {
                    return point.Height;
                  }
                }
              }
            }
          }
        }
      }

      return 0;
    }

    [BinarySchema]
    private partial class SchemaTilemapDefinition : IBiSerializable {
      private readonly byte padding_ = 0;

      public byte Unknown { get; private set; }

      public ushort Offset { get; private set; }
    }

    [BinarySchema]
    public partial class SchemaTile : IBiSerializable {
      public ushort[] Heights { get; } = new ushort[16];

      public Rgba32[] LightColors { get; } =
        Arrays.From(16, () => new Rgba32());

      public TileUvs[] SurfaceTextureUvsFromFirstRow { get; } =
        Arrays.From(4, () => new TileUvs());

      public TileUvs[] DetailTextureUvs { get; } =
        Arrays.From(16, () => new TileUvs());

      public uint MatlIndex { get; private set; }
    }

    [BinarySchema]
    public partial class TileUvs : IBiSerializable {
      public ushort U { get; private set; }
      public ushort V { get; private set; }
    }

    private class BwHeightmapChunk : IBwHeightmapChunk {
      public Grid<IBwHeightmapTile> Tiles { get; } = new(4, 4);
    }

    private class BwHeightmapTile : IBwHeightmapTile {
      public Grid<IBwHeightmapPoint> Points { get; } = new(4, 4);
      public uint MatlIndex { get; set; }

      public SchemaTile Schema { get; set; }
    }

    private class BwHeightmapPoint : IBwHeightmapPoint {
      public float X { get; set; }
      public float Y { get; set; }
      public float Height { get; set; }
      public Rgba32 LightColor { get; set; }
    }
  }
}