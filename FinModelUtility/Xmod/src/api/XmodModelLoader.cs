using System.Runtime.CompilerServices;

using BCnEncoder.Decoder;
using BCnEncoder.Shared;

using Dxt;

using fin.color;
using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;

using Pfim;

using SixLabors.ImageSharp.PixelFormats;

using xmod.schema;

using IImage = fin.image.IImage;
using PrimitiveType = xmod.schema.PrimitiveType;


namespace xmod.api {
  public class XmodModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.XmodFile;
    public required IFileHierarchyFile XmodFile { get; init; }
  }

  public class XmodModelLoader : IModelLoader<XmodModelFileBundle> {
    public IModel LoadModel(XmodModelFileBundle modelFileBundle) {
      using var tr =
          new FinTextReader(modelFileBundle.XmodFile.Impl.OpenRead());

      var xmod = new Xmod();
      xmod.Read(tr);

      var finModel = new ModelImpl();

      var finMaterialManager = finModel.MaterialManager;

      var finSkin = finModel.Skin;
      var finMesh = finSkin.AddMesh();

      var packetIndex = 0;
      foreach (var material in xmod.Materials) {
        IMaterial finMaterial;

        var textureIds = material.TextureIds;
        if (textureIds.Count == 0) {
          finMaterial = finMaterialManager.AddNullMaterial();
        } else {
          var textureId = textureIds[0];
          var textureName = textureId.Name;

          var textureFile = modelFileBundle
                            .XmodFile.Root.Impl.GetSubdir("texture_x")
                            .SearchForFiles($"{textureName}.tex", true)
                            .Single();

          using var er = new EndianBinaryReader(textureFile.OpenRead());
          var width = er.ReadUInt16();
          var height = er.ReadUInt16();
          var dxtType = er.ReadUInt16();

          ColorRgba32[] loadedDxt;

          IImage image;
          switch (dxtType) {
            // DXT1
            case 22: {
              var expectedLength = width * height / 16 * (2 + 2 + 4);

              er.Position = 0xe;
              var bytes = er.ReadBytes(expectedLength);

              loadedDxt =
                  new BcDecoder().DecodeRaw(bytes,
                                            width,
                                            height,
                                            CompressionFormat.Bc1);
              break;
            }
            // DXT3
            case 14: {
              var expectedLength = width * height / 16 * (8 + 2 + 2 + 4);

              er.Position = 0xe;
              var bytes = er.ReadBytes(expectedLength);

              loadedDxt =
                  new BcDecoder().DecodeRaw(bytes,
                                            width,
                                            height,
                                            CompressionFormat.Bc2);
              break;
            }
            // DXT5
            case 26: {
              var expectedLength = width * height / 16 * (8 + 2 + 2 + 4);

              er.Position = 0xe;
              var bytes = er.ReadBytes(expectedLength);

              loadedDxt =
                  new BcDecoder().DecodeRaw(bytes,
                                            width,
                                            height,
                                            CompressionFormat.Bc3);
              break;
            }
            default:
              throw new NotImplementedException();
          }

          unsafe {
            var rgbaImage = new Rgba32Image(width, height);
            image = rgbaImage;
            using var imageLock = rgbaImage.Lock();
            var ptr = imageLock.pixelScan0;

            for (var y = 0; y < height; y++) {
              for (var x = 0; x < width; ++x) {
                var i = y * width + x;

                var src = loadedDxt[i];
                ptr[i] = new Rgba32(src.r, src.g, src.b, src.a);
              }
            }
          }


          var finTexture = finMaterialManager.CreateTexture(image);
          finMaterial = finMaterialManager.AddTextureMaterial(finTexture);
        }

        for (var i = 0; i < material.NumPackets; ++i) {
          var packet = xmod.Packets[packetIndex];

          var packetVertices = packet.Adjuncts.Select(adjunct => {
                                       var position =
                                           xmod.Positions[
                                               adjunct.PositionIndex];
                                       var normal =
                                           xmod.Normals[adjunct.NormalIndex];
                                       var color =
                                           xmod.Colors[adjunct.ColorIndex];
                                       var uv1 = xmod.Uv1s[adjunct.Uv1Index];

                                       return finSkin
                                              .AddVertex(position.X,
                                                position.Y,
                                                position.Z)
                                              .SetLocalNormal(normal.X,
                                                normal.Y,
                                                normal.Z)
                                              .SetColor(
                                                  FinColor.FromRgbaFloats(
                                                      color.X,
                                                      color.Y,
                                                      color.Z,
                                                      color.W))
                                              .SetUv(uv1.X, uv1.Y);
                                     })
                                     .ToArray();

          foreach (var primitive in packet.Primitives) {
            var primitiveVertices =
                primitive.VertexIndices
                         .Skip(primitive.Type switch {
                             PrimitiveType.TRIANGLES => 0,
                             _                       => 1,
                         })
                         .Select(vertexIndex => packetVertices[vertexIndex])
                         .ToArray();
            var finPrimitive = primitive.Type switch {
                PrimitiveType.TRIANGLE_STRIP_1 => finMesh.AddTriangleStrip(
                    primitiveVertices),
                PrimitiveType.TRIANGLE_STRIP_2 => finMesh.AddTriangleStrip(
                    primitiveVertices),
                PrimitiveType.TRIANGLES =>
                    finMesh.AddTriangles(primitiveVertices),
            };

            finPrimitive.SetMaterial(finMaterial);

            if (primitive.Type == PrimitiveType.TRIANGLES) {
              finPrimitive.SetVertexOrder(VertexOrder.NORMAL);
            }
          }

          ++packetIndex;
        }
      }

      return finModel;
    }
  }
}