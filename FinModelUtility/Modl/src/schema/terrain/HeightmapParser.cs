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
      SchemaTilemapDefinition[] tilemapDefinitions;
      {
        using var tilemapEr =
            new EndianBinaryReader(new MemoryStream(tilemapBytes));
        tilemapEr.ReadNewArray(out tilemapDefinitions, 64 * 64);
      }

      SchemaTile[] schemaTiles;
      {
        using var tilesEr =
            new EndianBinaryReader(new MemoryStream(tilesBytes));
        var schemaTileCount = tilesBytes.Length / 180;
        tilesEr.ReadNewArray(out schemaTiles, schemaTileCount);
      }

      for (var chunkY = 0; chunkY < 64; ++chunkY) {
        for (var chunkX = 0; chunkX < 64; ++chunkX) {
          var tilemapDefinition = tilemapDefinitions[chunkY * 64 + chunkX];
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

      var values = new CounterArray[5];
      for (var i = 0; i < values.Length; ++i) {
        values[i] = new CounterArray();
      }

      foreach (var schemaTile in schemaTiles) {
        foreach (var frac in schemaTile.Unknowns0) {
          values[0].Increment((byte) frac);
        }

        var uvs = schemaTile.Uvs;
        for (var p = 0; p < uvs.Length; ++p) {
          var uv = uvs[p];

          for (var i = 0; i < 4; ++i) {
            values[1 + i].Increment(uv.Data[i]);
          }
        }

        ;
      }

      var lists = values.Select(counter => counter.ToArray()).ToArray();

      ;
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

      public BwUnknownEnum0[] Unknowns0 { get; } = new BwUnknownEnum0[16];

      public TileUvs[] Uvs { get; } = Arrays.From(16, () => new TileUvs());

      public uint MatlIndex { get; private set; }
    }

    public enum BwUnknownEnum0 : byte {
      VALUE_A = 0,
      VALUE_B = 16
    }

    [BinarySchema]
    public partial class TileUvs : IBiSerializable {
      public byte[] Data { get; } = new byte[4];
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
      public int X { get; set; }
      public int Y { get; set; }
      public ushort Height { get; set; }
      public Rgba32 LightColor { get; set; }
    }
  }
}