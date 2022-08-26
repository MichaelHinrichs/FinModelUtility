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

      var finMaterial =
          finModel.MaterialManager.AddTextureMaterial(
              finModel.MaterialManager.CreateTexture(
                  FinImage.Create1x1WithColor(Color.White)));

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      var triangles = new List<(IVertex, IVertex, IVertex)>();

      var chunks = bwHeightmap.Chunks;

      for (var chunkY = 0; chunkY < chunks.Height; ++chunkY) {
        for (var chunkX = 0; chunkX < chunks.Width; ++chunkX) {
          var tiles = chunks[chunkX, chunkY]?.Tiles;
          if (tiles == null) {
            continue;
          }

          var chunkWidth = 4 * 4;
          var chunkHeight = 4 * 4;
          var chunkFinVertices = new Grid<IVertex>(chunkWidth, chunkHeight);

          for (var tileY = 0; tileY < tiles.Height; ++tileY) {
            for (var tileX = 0; tileX < tiles.Width; ++tileX) {
              var points = tiles[tileX, tileY].Points;

              for (var pointY = 0; pointY < points.Height; ++pointY) {
                for (var pointX = 0; pointX < points.Width; ++pointX) {
                  var point = points[pointX, pointY];

                  var intensity = (byte) (point.Height / 24);

                  var finVertex =
                      finSkin.AddVertex(point.X, point.Height, point.Y)
                             .SetColor(ColorImpl.FromIntensityByte(intensity));

                  chunkFinVertices[4 * tileX + pointX, 4 * tileY + pointY] =
                      finVertex;
                }
              }
            }
          }

          for (var vY = 0; vY < chunkHeight - 1; ++vY) {
            for (var vX = 0; vX < chunkWidth - 1; ++vX) {
              var a = chunkFinVertices[vX, vY];
              var b = chunkFinVertices[vX + 1, vY];
              var c = chunkFinVertices[vX, vY + 1];
              var d = chunkFinVertices[vX + 1, vY + 1];

              triangles.Add((a, b, c));
              triangles.Add((d, c, b));
            }
          }
        }
      }

      finMesh.AddTriangles(triangles.ToArray()).SetMaterial(finMaterial);

      return finModel;
    }
  }
}