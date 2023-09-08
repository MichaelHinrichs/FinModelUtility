using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using Assimp;

using fin.color;
using fin.data.lazy;
using fin.data.queue;
using fin.image;
using fin.image.formats;
using fin.math.matrix.four;
using fin.math.rotations;
using fin.model.impl;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.model.io.importer.assimp {
  public class AssimpModelImporter : IModelImporter<AssimpModelFileBundle> {
    public unsafe IModel ImportModel(AssimpModelFileBundle modelFileBundle) {
      var mainFile = modelFileBundle.MainFile;

      var finModel = new ModelImpl();

      using var ctx = new AssimpContext();
      var assScene = ctx.ImportFile(mainFile.FullPath);

      // Adds materials
      var lazyFinSatelliteImages = new LazyDictionary<string, IImage>(
          path => mainFile.AssertGetParent()
                          .TryToGetExistingFile(path, out var imageFile)
              ? FinImage.FromFile(imageFile)
              : FinImage.Create1x1FromColor(Color.Magenta));
      var lazyFinEmbeddedImages = new LazyDictionary<EmbeddedTexture, IImage>(
          assEmbeddedImage => {
            if (assEmbeddedImage.IsCompressed) {
              using var stream =
                  new MemoryStream(assEmbeddedImage.CompressedData);
              return FinImage.FromStream(stream);
            }

            var finImage = new Rgba32Image(PixelFormat.RGBA8888,
                                           assEmbeddedImage.Width,
                                           assEmbeddedImage.Height);

            using var imgLock = finImage.Lock();
            for (var y = 0; y < finImage.Height; ++y) {
              for (var x = 0; x < finImage.Width; ++x) {
                var texel =
                    assEmbeddedImage.NonCompressedData[y * finImage.Width + x];
                imgLock.pixelScan0[y * finImage.Width + x] =
                    new Rgba32(texel.R, texel.G, texel.B, texel.A);
              }
            }

            return finImage;
          });
      var lazyFinTextures = new LazyDictionary<TextureSlot, ITexture?>(
          assTextureSlot => {
            var finImage = assTextureSlot.FilePath != null
                ? lazyFinSatelliteImages[assTextureSlot.FilePath]
                : lazyFinEmbeddedImages[
                    assScene.Textures[assTextureSlot.TextureIndex]];

            var finTexture = finModel.MaterialManager.CreateTexture(finImage);
            finTexture.WrapModeU = ConvertWrapMode_(assTextureSlot.WrapModeU);
            finTexture.WrapModeV = ConvertWrapMode_(assTextureSlot.WrapModeV);

            return finTexture;
          });
      var lazyFinMaterials = new LazyDictionary<int, IMaterial>(
          assMaterialIndex => {
            var assMaterial = assScene.Materials[assMaterialIndex];

            // TODO: Handle all billion properties within assMaterial
            var finMaterial = finModel
                              .MaterialManager.AddStandardMaterial();
            finMaterial.Name = assMaterial.Name;

            if (assMaterial.HasTextureDiffuse) {
              finMaterial.DiffuseTexture =
                  lazyFinTextures[assMaterial.TextureDiffuse];
            }

            if (assMaterial.HasTextureNormal) {
              finMaterial.NormalTexture =
                  lazyFinTextures[assMaterial.TextureNormal];
            }

            if (assMaterial.HasTextureAmbientOcclusion) {
              finMaterial.AmbientOcclusionTexture =
                  lazyFinTextures[assMaterial.TextureAmbientOcclusion];
            }

            if (assMaterial.HasTextureSpecular) {
              finMaterial.SpecularTexture =
                  lazyFinTextures[assMaterial.TextureSpecular];
            }

            if (assMaterial.HasTextureEmissive) {
              finMaterial.EmissiveTexture =
                  lazyFinTextures[assMaterial.TextureEmissive];
            }

            return finMaterial;
          });

      // Adds rig
      var finSkeleton = finModel.Skeleton;
      var nodeAndBoneQueue =
          new FinQueue<(Node, IBone)>((assScene.RootNode, finSkeleton.Root));
      while (nodeAndBoneQueue.TryDequeue(out var nodeAndBone)) {
        var (assNode, finBone) = nodeAndBone;
        finBone.Name = assNode.Name;

        var assTransform = assNode.Transform;
        var finMatrix =
            new FinMatrix4x4(
                Unsafe.As<Assimp.Matrix4x4, System.Numerics.Matrix4x4>(
                    ref assTransform));
        finMatrix.Decompose(out var translation,
                            out var rotation,
                            out var scale);
        var eulerRadians = QuaternionUtil.ToEulerRadians(rotation);

        finBone.SetLocalPosition(translation.X, translation.Y, translation.Z)
               .SetLocalRotationRadians(eulerRadians.X,
                                        eulerRadians.Y,
                                        eulerRadians.Z)
               .SetLocalScale(scale.X, scale.Y, scale.Z);

        nodeAndBoneQueue.Enqueue(
            assNode.Children.Select(childNode
                                        => (childNode,
                                            finBone.AddChild(0, 0, 0))));
      }

      // Adds skin
      var finSkin = finModel.Skin;
      foreach (var assMesh in assScene.Meshes) {
        var finMesh = finSkin.AddMesh();
        finMesh.Name = assMesh.Name;

        var finVertices =
            assMesh.Vertices
                   .Select(assPosition => finSkin.AddVertex(
                               assPosition.X,
                               assPosition.Y,
                               assPosition.Z))
                   .ToArray();

        if (assMesh.HasNormals) {
          for (var i = 0; i < finVertices.Length; ++i) {
            var assNormal = assMesh.Normals[i];
            finVertices[i]
                .SetLocalNormal(assNormal.X, assNormal.Y, assNormal.Z);
          }
        }

        for (var colorIndex = 0;
             colorIndex < assMesh.VertexColorChannelCount;
             colorIndex++) {
          if (!assMesh.HasVertexColors(colorIndex)) {
            continue;
          }

          var assColors = assMesh.VertexColorChannels[colorIndex];
          for (var i = 0; i < finVertices.Length; ++i) {
            var assColor = assColors[i];
            finVertices[i]
                .SetColor(colorIndex,
                          FinColor.FromRgbaFloats(
                              assColor.R,
                              assColor.G,
                              assColor.B,
                              assColor.A));
          }
        }

        for (var uvIndex = 0;
             uvIndex < assMesh.TextureCoordinateChannelCount;
             uvIndex++) {
          if (!assMesh.HasTextureCoords(uvIndex)) {
            continue;
          }

          var assUvs = assMesh.TextureCoordinateChannels[uvIndex];
          for (var i = 0; i < finVertices.Length; ++i) {
            var assUv = assUvs[i];
            finVertices[i].SetUv(uvIndex, assUv.X, assUv.Y);
          }
        }

        var faceVertices =
            assMesh.GetIndices().Select(i => finVertices[i]).ToArray();
        var finPrimitive = assMesh.PrimitiveType switch {
            Assimp.PrimitiveType.Point    => finMesh.AddPoints(faceVertices),
            Assimp.PrimitiveType.Line     => finMesh.AddLines(faceVertices),
            Assimp.PrimitiveType.Triangle => finMesh.AddTriangles(faceVertices),
        };

        finPrimitive.SetMaterial(lazyFinMaterials[assMesh.MaterialIndex]);
      }

      return finModel;
    }

    private static WrapMode ConvertWrapMode_(TextureWrapMode assWrapMode)
      => assWrapMode switch {
          TextureWrapMode.Wrap   => WrapMode.REPEAT,
          TextureWrapMode.Clamp  => WrapMode.CLAMP,
          TextureWrapMode.Mirror => WrapMode.MIRROR_REPEAT,
      };
  }
}