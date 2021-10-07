using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;

using fin.io;
using fin.log;
using fin.model;
using fin.util.asserts;

using Microsoft.Extensions.Logging;

using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using AlphaMode = SharpGLTF.Materials.AlphaMode;

namespace fin.exporter.gltf {
  public class GltfExporter : IExporter {
    private readonly ILogger logger_ = Logging.Create<GltfExporter>();

    public bool UvIndices { get; set; }
    public bool Embedded { get; set; }

    public void Export(IFile outputFile, IModel model) {
      Asserts.True(
          outputFile.Extension.EndsWith(".gltf") ||
          outputFile.Extension.EndsWith(".glb"),
          "Target file is not a GLTF format!");

      this.logger_.BeginScope("Export");

      var modelRoot = ModelRoot.CreateModel();

      var scene = modelRoot.UseScene("default");
      var skin = modelRoot.CreateSkin();

      var animations = model.AnimationManager.Animations;
      var firstAnimation = (animations?.Count ?? 0) > 0 ? animations[0] : null;

      if (firstAnimation != null) {
        // TODO: Put this somewhere else!
        // We should be able to use the raw bone positions, but this screws up
        // bones with multiple weights for some reason, perhaps because the
        // model is contorted in an unnatural way? Anyway, we NEED to use the
        // first animation instead.
        //this.ApplyFirstFrameToSkeleton_(model.Skeleton, firstAnimation);
      }

      // Builds skeleton.
      var rootNode = scene.CreateNode();
      var skinNodeAndBones = new GltfSkeletonBuilder().BuildAndBindSkeleton(
          rootNode,
          skin,
          model.Skeleton);

      // Builds animations.
      new GltfAnimationBuilder().BuildAnimations(
          modelRoot,
          skinNodeAndBones,
          model.AnimationManager.Animations);

      // Builds materials.
      // TODO: Update this if GLTF is ever extended...
      var finToTexCoordAndGltfMaterial =
          new Dictionary<IMaterial, (IList<byte>, MaterialBuilder)>();
      {
        foreach (var finMaterial in model.MaterialManager.All) {
          var gltfMaterial = new MaterialBuilder(finMaterial.Name)
                             .WithDoubleSide(false)
                             .WithSpecularGlossinessShader()
                             .WithSpecularGlossiness(new Vector3(0), 0);

          // TODO: Simplify/cut down on redundant logic
          var hasTexture = finMaterial.Textures.Count > 0;
          if (hasTexture && finMaterial is ILayerMaterial layerMaterial) {
            var addLayers =
                layerMaterial
                    .Layers
                    .Where(layer => layer.BlendMode == BlendMode.ADD)
                    .ToArray();
            var multiplyLayers =
                layerMaterial
                    .Layers
                    .Where(layer => layer.BlendMode == BlendMode.MULTIPLY)
                    .ToArray();

            if (addLayers.Length == 0) {
              //throw new NotSupportedException("Expected to find an add layer!");
            }
            if (addLayers.Length > 1) {
              ;
            }
            if (addLayers.Length > 2) {
              //throw new NotSupportedException("Too many add layers for GLTF!");
            }

            /*var lastLayer = addLayers.Any()
                                ? addLayers.First()
                                : multiplyLayers.First();*/

            var channels = new[] {KnownChannel.Diffuse, KnownChannel.Emissive};

            var textureCoordIndices = new List<byte>();
            for (var i = 0; i < Math.Min(addLayers.Length, 2); ++i) {
              var layer = addLayers[i];

              var texture = layer.ColorSource as ITexture;
              var textureBuilder = gltfMaterial.UseChannel(channels[i])
                                               .UseTexture();

              var imageStream = new MemoryStream();
              texture.ImageData.Save(imageStream, ImageFormat.Png);
              var imageBytes = imageStream.ToArray();
              var memoryImage = new MemoryImage(imageBytes);

              textureBuilder.WithPrimaryImage(memoryImage)
                            .WithCoordinateSet(0)
                            .WithSampler(
                                this.ConvertWrapMode_(texture.WrapModeU),
                                this.ConvertWrapMode_(texture.WrapModeV));

              textureCoordIndices.Add(layer.TexCoordIndex);
            }

            finToTexCoordAndGltfMaterial[finMaterial] =
                (textureCoordIndices, gltfMaterial);
          } else if (hasTexture &&
                     finMaterial is ITextureMaterial textureMaterial) {
            var texture = textureMaterial.Texture;
            var textureBuilder = gltfMaterial.UseChannel(KnownChannel.Diffuse)
                                             .UseTexture();

            var imageStream = new MemoryStream();
            texture.ImageData.Save(imageStream, ImageFormat.Png);
            var imageBytes = imageStream.ToArray();
            var memoryImage = new MemoryImage(imageBytes);

            textureBuilder.WithPrimaryImage(memoryImage)
                          .WithCoordinateSet(0)
                          .WithSampler(
                              this.ConvertWrapMode_(texture.WrapModeU),
                              this.ConvertWrapMode_(texture.WrapModeV));

            finToTexCoordAndGltfMaterial[finMaterial] =
                (new byte[] { 0 }, gltfMaterial);
          } else {
            finToTexCoordAndGltfMaterial[finMaterial] =
                (new byte[] {0}, gltfMaterial);
          }

          /*var hasTexture = finMaterial.Textures.Count > 0;
          if (hasTexture) {
            gltfMaterial.WithAlpha(AlphaMode.MASK);

            var lastTexture = finMaterial.Textures.Last();
            var textureBuilder = gltfMaterial.UseChannel(KnownChannel.Diffuse)
                                             .UseTexture();

            var imageStream = new MemoryStream();
            lastTexture.ImageData.Save(imageStream, ImageFormat.Png);
            var imageBytes = imageStream.ToArray();
            var memoryImage = new MemoryImage(imageBytes);

            textureBuilder.WithPrimaryImage(memoryImage)
                          .WithCoordinateSet(0)
                          .WithSampler(
                              this.ConvertWrapMode_(lastTexture.WrapModeU),
                              this.ConvertWrapMode_(lastTexture.WrapModeV));
          }*/
        }
      }

      // Builds mesh.
      var meshBuilder = new GltfMeshBuilder {UvIndices = this.UvIndices};
      var mesh = meshBuilder.BuildAndBindMesh(
          modelRoot,
          model,
          finToTexCoordAndGltfMaterial);

      scene.CreateNode()
           .WithSkinnedMesh(mesh,
                            rootNode.WorldMatrix,
                            skinNodeAndBones
                                .Select(
                                    skinNodeAndBone => skinNodeAndBone.Item1)
                                .ToArray());

      var writeSettings = new WriteSettings {
          ImageWriting = this.Embedded
                             ? ResourceWriteMode.Embedded
                             : ResourceWriteMode.SatelliteFile,
      };

      var outputPath = outputFile.FullName;
      this.logger_.LogInformation($"Writing to {outputPath}...");
      modelRoot.Save(outputPath, writeSettings);
    }

    // TODO: Pull this out somewhere else, or make this part of the model creation flow?
    private void ApplyFirstFrameToSkeleton_(
        ISkeleton skeleton,
        IAnimation animation) {
      var boneQueue = new Queue<IBone>();
      boneQueue.Enqueue(skeleton.Root);

      while (boneQueue.Count > 0) {
        var bone = boneQueue.Dequeue();

        animation.BoneTracks.TryGetValue(bone, out var boneTracks);

        var localPosition = boneTracks?.Positions.GetInterpolatedFrame(0);
        bone.SetLocalPosition(localPosition?.X ?? 0,
                              localPosition?.Y ?? 0,
                              localPosition?.Z ?? 0);

        var localRotation = boneTracks?.Rotations.GetAlmostKeyframe(0);
        bone.SetLocalRotationRadians(localRotation?.XRadians ?? 0,
                                     localRotation?.YRadians ?? 0,
                                     localRotation?.ZRadians ?? 0);

        // It seems like some animations shrink a bone to 0 to hide it, but
        // this prevents us from calculating a determinant to invert the
        // matrix. As a result, we can't safely include the scale here.
        IScale?
            localScale = null; //boneTracks?.Scales.GetInterpolatedAtFrame(0);
        bone.SetLocalScale(localScale?.X ?? 1,
                           localScale?.Y ?? 1,
                           localScale?.Z ?? 1);

        foreach (var child in bone.Children) {
          boneQueue.Enqueue(child);
        }
      }
    }

    private TextureWrapMode ConvertWrapMode_(WrapMode wrapMode)
      => wrapMode switch {
          WrapMode.CLAMP         => TextureWrapMode.CLAMP_TO_EDGE,
          WrapMode.REPEAT        => TextureWrapMode.REPEAT,
          WrapMode.MIRROR_REPEAT => TextureWrapMode.MIRRORED_REPEAT,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(wrapMode),
                   wrapMode,
                   null)
      };
  }
}