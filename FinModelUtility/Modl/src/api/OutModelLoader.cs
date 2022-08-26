using System.Collections;
using System.Drawing;
using System.IO.Compression;

using fin.data;
using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;

using modl.schema.modl;
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

      var bwTerrain =
          isBw2
              ? (IBwTerrain) er.ReadNew<Bw2Terrain>()
              : er.ReadNew<Bw1Terrain>();

      var finModel = new ModelImpl();

      var textureDictionary = new LazyDictionary<string, ITexture>(
          textureName => {
            var outFile = modelFileBundle.OutFile;
            var outDirectory =
                outFile.Parent.Subdirs.Single(
                    dir => dir.Name == outFile.NameWithoutExtension + "_Level");

            var textureFile =
                outDirectory.Files.Single(
                    file => file.Name.ToLower() == $"{textureName}.png");

            var image = FinImage.FromFile(textureFile.Impl);

            var finTexture =
                finModel.MaterialManager.CreateTexture(image);
            finTexture.Name = textureName;

            // TODO: Need to handle wrapping
            finTexture.WrapModeU = WrapMode.REPEAT;
            finTexture.WrapModeV = WrapMode.REPEAT;

            return finTexture;
          });
      var materialDictionary =
          new LazyDictionary<uint, IMaterial>(matlIndex => {
            var matl = bwTerrain.Materials[(int) matlIndex];

            var texture1 = textureDictionary[matl.Texture1];
            var texture2 = textureDictionary[matl.Texture2];

            var finMaterial =
                finModel.MaterialManager.AddFixedFunctionMaterial();

            var equations = finMaterial.Equations;
            var color0 = equations.CreateColorConstant(0);
            var scalar1 = equations.CreateScalarConstant(1);
            equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR,
                                        equations.CreateColorInput(
                                            FixedFunctionSource.VERTEX_COLOR_0,
                                            color0));
            equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                         scalar1);

            return finMaterial;
          });

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      var bwHeightmap = bwTerrain.Heightmap;
      var chunks = bwHeightmap.Chunks;

      var tileSize = 64 * 4;
      //var tileMaterials = new Grid<IMaterial?>(tileSize, tileSize);

      var tileMatlIndices = new Grid<uint?>(tileSize, tileSize);

      var heightmapSize = tileSize * 4;
      var chunkFinVertices = new Grid<IVertex?>(heightmapSize, heightmapSize);

      for (var chunkY = 0; chunkY < chunks.Height; ++chunkY) {
        for (var chunkX = 0; chunkX < chunks.Width; ++chunkX) {
          var tiles = chunks[chunkX, chunkY]?.Tiles;
          if (tiles == null) {
            continue;
          }

          for (var tileY = 0; tileY < tiles.Height; ++tileY) {
            for (var tileX = 0; tileX < tiles.Width; ++tileX) {
              var tile = tiles[tileX, tileY];
              /*tileMaterials[4 * chunkX + tileX, 4 * chunkY + tileY] =
                  materialDictionary[tile.MatlIndex];*/
              tileMatlIndices[4 * chunkX + tileX, 4 * chunkY + tileY] =
                  tile.MatlIndex;

              var points = tile.Points;
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
                             .SetUv(1f * heightmapX / heightmapSize,
                                    1f * heightmapY / heightmapSize);

                  chunkFinVertices[heightmapX, heightmapY] = finVertex;
                }
              }
            }
          }
        }
      }

      {
        /*var minTx = tileSize;
        var maxTx = -1;
        var minTy = tileSize;
        var maxTy = -1;
        for (var tY = 0; tY < tileSize; ++tY) {
          for (var tX = 0; tX < tileSize; ++tX) {
            var matlIndexOrNull = tileMatlIndices[tX, tY];
            if (matlIndexOrNull == null) {
              continue;
            }

            minTx = Math.Min(minTx, tX);
            maxTx = Math.Max(maxTx, tX);
            minTy = Math.Min(minTy, tY);
            maxTy = Math.Max(maxTy, tY);
          }
        }

        var tWidth = (maxTx - minTx) + 1;
        var tHeight = (maxTy - minTy) + 1;

        var resolution = 32;
        var debugImage =
            new Rgba32Image(tWidth * resolution, tHeight * resolution);

        var nullImage1 =
            FinImage.CreateFromColor(Color.Magenta, resolution, resolution);
        var nullImage2 =
            FinImage.CreateFromColor(Color.Aqua, resolution, resolution);

        debugImage.Mutate((_, setHandler) => {
          for (var tY = minTy; tY <= maxTy; ++tY) {
            for (var tX = minTx; tX <= maxTx; ++tX) {
              var matlIndexOrNull = tileMatlIndices[tX, tY];
              if (matlIndexOrNull == null) {
                continue;
              }

              BwHeightmapMaterial? matl = null;

              var matlIndex = matlIndexOrNull.Value;
              if (matlIndex < bwTerrain.Materials.Count) {
                matl = bwTerrain.Materials[(int) matlIndex];
              }

              var image1 = matl != null
                               ? textureDictionary[matl.Texture1].Image
                               : nullImage1;
              var image2 = matl != null
                               ? textureDictionary[matl.Texture2].Image
                               : nullImage2;

              Action<Action<IImage.Rgba32GetHandler, IImage.Rgba32GetHandler>>
                  bothGets;
              if (image1 == image2) {
                bothGets = handler => {
                  image1.Access(getHandler1 => {
                    handler(getHandler1, getHandler1);
                  });
                };
              } else {
                bothGets = handler => {
                  image1.Access(getHandler1 => {
                    image2.Access(getHandler2 => {
                      handler(getHandler1, getHandler2);
                    });
                  });
                };
              }

              bothGets((getHandler1, getHandler2) => {
                for (var iY = 0; iY < resolution; ++iY) {
                  for (var iX = 0; iX < resolution; ++iX) {
                    var x = (tX - minTx) * resolution + iX;
                    var y = (tY - minTy) * resolution + iY;

                    byte r, g, b, a;

                    var useFirstImage = (iX + iY) < resolution;
                    if (useFirstImage) {
                      var adjX =
                          (int) ((1f * iX / resolution) *
                                 image1.Width);
                      var adjY =
                          (int) ((1f * iY / resolution) *
                                 image1.Height);

                      getHandler1(adjX, adjY, out r, out g, out b,
                                  out a);
                    } else {
                      var adjX =
                          (int) ((1f * iX / resolution) *
                                 image2.Width);
                      var adjY =
                          (int) ((1f * iY / resolution) *
                                 image2.Height);

                      getHandler2(adjX, adjY, out r, out g, out b,
                                  out a);
                    }

                    setHandler(x, y, r, g, b, a);
                  }
                }
              });
            }
          }
        });
        debugImage.AsBitmap()
                  .Save(
                      @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test\image.png");*/
      }

      var triangles = new List<(IVertex, IVertex, IVertex)>();

      for (var vY = 0; vY < heightmapSize - 1; ++vY) {
        for (var vX = 0; vX < heightmapSize - 1; ++vX) {
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

      var finMaterial =
          finModel.MaterialManager.AddFixedFunctionMaterial();

      var equations = finMaterial.Equations;
      var color0 = equations.CreateColorConstant(0);
      var scalar1 = equations.CreateScalarConstant(1);
      equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR,
                                  equations.CreateColorInput(
                                      FixedFunctionSource.VERTEX_COLOR_0,
                                      color0));
      equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                   scalar1);

      finMesh.AddTriangles(triangles.ToArray()).SetMaterial(finMaterial);

      return finModel;
    }
  }
}