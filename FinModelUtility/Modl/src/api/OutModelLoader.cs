using System.Drawing;
using System.IO.Compression;

using fin.data;
using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;

using modl.schema.terrain;
using modl.schema.terrain.bw1;


namespace modl.api {
  public class OutModelFileBundle : IBattalionWarsModelFileBundle {
    public IFileHierarchyFile MainFile => this.OutFile;

    public GameVersion GameVersion { get; set; }
    public IFileHierarchyFile OutFile { get; set; }
  }

  public class OutModelLoader : IModelLoader<OutModelFileBundle> {
    public IModel LoadModel(OutModelFileBundle modelFileBundle) {
      var outFile = modelFileBundle.OutFile;

      var isBw2 = modelFileBundle.GameVersion == GameVersion.BW2;

      Stream stream;
      if (isBw2) {
        using var gZipStream =
            new GZipStream(outFile.Impl.OpenRead(),
                           CompressionMode.Decompress);

        stream = new MemoryStream();
        gZipStream.CopyTo(stream);
        stream.Position = 0;
      } else {
        stream = outFile.Impl.OpenRead();
      }

      using var er = new EndianBinaryReader(stream, Endianness.LittleEndian);

      var bwHeightmap =
          isBw2
              ? (IBwHeightmap) er.ReadNew<Bw2Heightmap>()
              : er.ReadNew<Bw1Heightmap>();

      var finModel = new ModelImpl();

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      var triangles = new List<(IVertex, IVertex, IVertex)>();

      var chunks = bwHeightmap.Chunks;

      var heightmapWidth = 64 * 4 * 4;
      var heightmapHeight = 64 * 4 * 4;
      var chunkFinVertices =
          new Grid<IVertex?>(heightmapWidth, heightmapHeight);

      var heights = new Grid<ushort>(heightmapWidth, heightmapHeight);

      for (var chunkY = 0; chunkY < chunks.Height; ++chunkY) {
        for (var chunkX = 0; chunkX < chunks.Width; ++chunkX) {
          var tiles = chunks[chunkX, chunkY]?.Tiles;
          if (tiles == null) {
            continue;
          }

          for (var tileY = 0; tileY < tiles.Height; ++tileY) {
            for (var tileX = 0; tileX < tiles.Width; ++tileX) {
              var points = tiles[tileX, tileY].Points;

              for (var pointY = 0; pointY < points.Height; ++pointY) {
                for (var pointX = 0; pointX < points.Width; ++pointX) {
                  var point = points[pointX, pointY];

                  var heightmapX = 16 * chunkX + 4 * tileX + pointX;
                  var heightmapY = 16 * chunkY + 4 * tileY + pointY;

                  var finVertex =
                      finSkin.AddVertex(point.X, point.Height, point.Y)
                             .SetUv(1f * heightmapX / heightmapWidth,
                                    1f * heightmapY / heightmapHeight);

                  chunkFinVertices[heightmapX, heightmapY] = finVertex;
                  heights[heightmapX, heightmapY] = point.Height;
                }
              }
            }
          }
        }
      }

      var image = new I8Image(heightmapWidth, heightmapHeight);
      image.Mutate((_, setHandler) => {
        for (var vY = 0; vY < heightmapHeight; ++vY) {
          for (var vX = 0; vX < heightmapWidth; ++vX) {
            setHandler(vX, vY, (byte) (heights[vX, vY] / 24));
          }
        }
      });
      var heightmapTexture = finModel.MaterialManager.CreateTexture(image);
      var finMaterial =
          finModel.MaterialManager.AddTextureMaterial(heightmapTexture);

      for (var vY = 0; vY < heightmapHeight - 1; ++vY) {
        for (var vX = 0; vX < heightmapWidth - 1; ++vX) {
          var a = chunkFinVertices[vX, vY];
          var b = chunkFinVertices[vX + 1, vY];
          var c = chunkFinVertices[vX, vY + 1];
          var d = chunkFinVertices[vX + 1, vY + 1];

          if (a != null && b != null && c != null && d != null) {
            triangles.Add((a, b, c));
            triangles.Add((d, c, b));
          }
        }
      }

      finMesh.AddTriangles(triangles.ToArray()).SetMaterial(finMaterial);

      return finModel;
    }
  }
}