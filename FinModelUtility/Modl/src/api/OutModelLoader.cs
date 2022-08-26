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

                  var lightColor = point.LightColor;

                  var finVertex =
                      finSkin.AddVertex(point.X, point.Height, point.Y)
                             .SetColor(
                                 ColorImpl.FromRgbBytes(
                                     lightColor.R, lightColor.G, lightColor.B))
                             .SetUv(1f * heightmapX / heightmapWidth,
                                    1f * heightmapY / heightmapHeight);

                  chunkFinVertices[heightmapX, heightmapY] = finVertex;
                }
              }
            }
          }
        }
      }

      var lightImage = new Rgb24Image(heightmapWidth, heightmapHeight);
      lightImage.Mutate((_, setHandler) => {
        for (var vY = 0; vY < heightmapHeight; ++vY) {
          for (var vX = 0; vX < heightmapWidth; ++vX) {
            var finVertex = chunkFinVertices[vX, vY];

            if (finVertex == null) {
              continue;
            }

            var lightColor = finVertex.GetColor();
            setHandler(vX, vY, lightColor.Rb, lightColor.Gb, lightColor.Bb);
          }
        }
      });
      var lightTexture = finModel.MaterialManager.CreateTexture(lightImage);

      var finMaterial =
          finModel.MaterialManager.AddTextureMaterial(lightTexture);

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