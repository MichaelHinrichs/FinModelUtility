using System.Collections;
using System.Drawing;
using System.IO.Compression;

using fin.data;
using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;
using fin.util.color;

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

            finMaterial.SetTextureSource(0, texture1);
            finMaterial.SetTextureSource(1, texture2);

            var equations = finMaterial.Equations;
            var color0 = equations.CreateColorConstant(0);
            var scalar1 = equations.CreateScalarConstant(1);

            var vertexColor0 = equations.CreateColorInput(
                FixedFunctionSource.VERTEX_COLOR_0,
                color0);

            var textureColor1 = equations.CreateColorInput(
                FixedFunctionSource.TEXTURE_COLOR_0,
                color0);
            var textureColor2 = equations.CreateColorInput(
                FixedFunctionSource.TEXTURE_COLOR_1,
                color0);

            var blendedTextureColor = textureColor1;

            equations.CreateColorOutput(
                FixedFunctionSource.OUTPUT_COLOR,
                vertexColor0.Multiply(blendedTextureColor));
            equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                         scalar1);

            return finMaterial;
          });

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      var bwHeightmap = bwTerrain.Heightmap;
      var chunks = bwHeightmap.Chunks;

      var chunkCountX = chunks.Width;
      var chunkCountY = chunks.Height;

      var tileCountX = chunkCountX * 4;
      var tileCountY = chunkCountY * 4;

      var tileGrid = new Grid<IBwHeightmapTile?>(tileCountX, tileCountY);

      var heightmapSizeX = tileCountX * 4;
      var heightmapSizeY = tileCountY * 4;
      var chunkFinVertices = new Grid<IVertex?>(heightmapSizeX, heightmapSizeY);

      var trianglesByMaterial =
          new ListDictionary<IMaterial, (IVertex, IVertex, IVertex)>();

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
              tileGrid[4 * chunkX + tileX, 4 * chunkY + tileY] = tile;

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
                                     lightColor.R, lightColor.G, lightColor.B));

                  chunkFinVertices[heightmapX, heightmapY] = finVertex;
                }
              }

              var material = materialDictionary[tile.MatlIndex];
              for (var pointY = 0; pointY < points.Height - 1; ++pointY) {
                for (var pointX = 0; pointX < points.Width - 1; ++pointX) {
                  var vX = 16 * chunkX + 4 * tileX + pointX;
                  var vY = 16 * chunkY + 4 * tileY + pointY;

                  var a = chunkFinVertices[vX, vY];
                  var b = chunkFinVertices[vX + 1, vY];
                  var c = chunkFinVertices[vX, vY + 1];
                  var d = chunkFinVertices[vX + 1, vY + 1];

                  if (a != null && b != null && c != null && d != null) {
                    trianglesByMaterial.Add(material, (a, b, c));
                    trianglesByMaterial.Add(material, (d, c, b));
                  }
                }
              }
            }
          }
        }
      }

      foreach (var (material, triangles) in trianglesByMaterial) {
        finMesh.AddTriangles(triangles.ToArray()).SetMaterial(material);
      }

      /*
      {
        var minTx = tileCountX;
        var maxTx = -1;
        var minTy = tileCountY;
        var maxTy = -1;
        for (var tY = 0; tY < tileCountY; ++tY) {
          for (var tX = 0; tX < tileCountX; ++tX) {
            var tile = tileGrid[tX, tY];
            if (tile == null) {
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
              var tile = tileGrid[tX, tY];
              if (tile == null) {
                continue;
              }

              var matlIndex = tile.MatlIndex;
              var matl = bwTerrain.Materials[(int) matlIndex];
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
                    var pX = (int) Math.Floor((1f * iX / resolution) * 4);
                    var pY = (int) Math.Floor((1f * iY / resolution) * 4);
                    var pI = 4 * pY + pX;

                    var x = (tX - minTx) * resolution + iX;
                    var y = (tY - minTy) * resolution + iY;

                    var unk0Int = tile.Schema.Uvs[pI].Data[0];
                    var unk0Frac = unk0Int / 16f;

                    // Generally results in vertical bars across image, seems to be U?
                    var unk1Int = tile.Schema.Uvs[pI].Data[1];
                    var unk1Frac = unk1Int / 16f;

                    // Results in bars steadily increasing in either direction, but closer to unk3
                    var unk2Int = tile.Schema.Uvs[pI].Data[2];
                    var unk2Frac = unk2Int / 16f;

                    // Generally results in horizontal bars across image, seems to be V?
                    var unk3Int = tile.Schema.Uvs[pI].Data[3];
                    var unk3Frac = unk3Int / 255f;

                    var blendFrac =
                        tile.Schema.Unknowns0[pI] ==
                        HeightmapParser.BwUnknownEnum0.VALUE_A
                            ? 0
                            : 1;

                    var xF = 1f * iX / resolution;
                    var yF = 1f * iY / resolution;

                    var adj1X = (int) (xF * image1.Width);
                    var adj1Y = (int) (yF * image1.Height);

                    var adj2X = (int) (xF * image2.Width);
                    var adj2Y = (int) (yF * image2.Height);

                    getHandler1(adj1X, adj1Y,
                                out var r1,
                                out var g1,
                                out var b1,
                                out var a1);
                    getHandler2(adj2X, adj2Y,
                                out var r2,
                                out var g2,
                                out var b2,
                                out var a2);

                    ColorUtil.Interpolate(
                        r1, g1, b1, a1,
                        r2, g2, b2, a2, blendFrac,
                        out var r, out var g, out var b, out var a);

                    setHandler(x, y, r, g, b, a);
                  }
                }
              });
            }
          }
        });
        debugImage.AsBitmap()
                  .Save(
                      @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test\image.png");
      }
      */

      /*var triangles = new List<(IVertex, IVertex, IVertex)>();

      for (var vY = 0; vY < heightmapSizeY - 1; ++vY) {
        for (var vX = 0; vX < heightmapSizeX - 1; ++vX) {
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

      finMesh.AddTriangles(triangles.ToArray()).SetMaterial(finMaterial);*/

      return finModel;
    }
  }
}