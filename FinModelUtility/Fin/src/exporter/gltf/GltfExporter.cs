using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;

using fin.image;
using fin.io;
using fin.log;
using fin.model;
using fin.model.util;
using fin.util.asserts;
using fin.util.image;

using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using AlphaMode = SharpGLTF.Materials.AlphaMode;


namespace fin.exporter.gltf {
  public interface IGltfExporter : IExporter {
    bool UvIndices { get; set; }
    bool Embedded { get; set; }

    ModelRoot CreateModelRoot(IModel model);
  }

  public class GltfExporter : IGltfExporter {
    private readonly ILogger logger_ = Logging.Create<GltfExporter>();

    public bool UvIndices { get; set; }
    public bool Embedded { get; set; }

    public ModelRoot CreateModelRoot(IModel model) {
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
                             .WithDoubleSide(finMaterial.CullingMode switch {
                               CullingMode.SHOW_FRONT_ONLY => false,
                               // Darn, guess we can't support this.
                               CullingMode.SHOW_BACK_ONLY => true,
                               CullingMode.SHOW_BOTH => true,
                               // Darn, guess we can't support this either.
                               CullingMode.SHOW_NEITHER => false,
                               _ => throw new ArgumentOutOfRangeException()
                             })
                             .WithSpecularGlossinessShader()
                             .WithSpecularGlossiness(new Vector3(0), 0);

          switch (finMaterial) {
            case IStandardMaterial standardMaterial: {
                var diffuseTexture = standardMaterial.DiffuseTexture;
                if (diffuseTexture != null) {
                  gltfMaterial
                      .UseChannel(KnownChannel.Diffuse)
                      .UseTexture()
                      .WithPrimaryImage(
                          GltfExporter
                              .GetGltfImageFromFinTexture_(diffuseTexture));
                }

                var normalTexture = standardMaterial.NormalTexture;
                if (normalTexture != null) {
                  gltfMaterial.UseChannel(KnownChannel.Normal)
                              .UseTexture()
                              .WithPrimaryImage(
                                  GltfExporter.GetGltfImageFromFinTexture_(
                                      normalTexture));
                }

                var emissiveTexture = standardMaterial.EmissiveTexture;
                if (emissiveTexture != null) {
                  gltfMaterial.UseChannel(KnownChannel.Emissive)
                              .UseTexture()
                              .WithPrimaryImage(
                                  GltfExporter.GetGltfImageFromFinTexture_(
                                      emissiveTexture));
                }

                /*var specularTexture = standardMaterial.SpecularTexture;
                if (specularTexture != null) {
                  gltfMaterial.WithSpecularGlossiness(
                      GltfExporter.GetGltfImageFromFinTexture_(
                          specularTexture), new Vector3(.1f), .1f);
                }*/

                var ambientOcclusionTexture =
                    standardMaterial.AmbientOcclusionTexture;
                if (ambientOcclusionTexture != null) {
                  gltfMaterial
                      .UseChannel(KnownChannel.Occlusion)
                      .UseTexture()
                      .WithPrimaryImage(
                          GltfExporter
                              .GetGltfImageFromFinTexture_(
                                  ambientOcclusionTexture));
                }

                break;
              }
            default: {
                var texture = PrimaryTextureFinder.GetFor(finMaterial);
                if (texture != null) {
                  var alphaMode = texture.TransparencyType switch {
                    ImageTransparencyType.OPAQUE => AlphaMode.OPAQUE,
                    ImageTransparencyType.MASK => AlphaMode.MASK,
                    ImageTransparencyType.TRANSPARENT => AlphaMode.BLEND,
                    _ => throw new ArgumentOutOfRangeException()
                  };
                  gltfMaterial.WithAlpha(alphaMode);

                  gltfMaterial
                      .UseChannel(KnownChannel.Diffuse)
                      .UseTexture()
                      .WithPrimaryImage(
                          GltfExporter.GetGltfImageFromFinTexture_(texture))
                      .WithCoordinateSet(0)
                      .WithSampler(
                          this.ConvertWrapMode_(texture.WrapModeU),
                          this.ConvertWrapMode_(texture.WrapModeV));
                }
                break;
              }
          }

          finToTexCoordAndGltfMaterial[finMaterial] =
              (new byte[] { 0 }, gltfMaterial);
        }
      }

      // Builds meshes.
      var meshBuilder = new GltfMeshBuilder { UvIndices = this.UvIndices };
      var gltfMeshes = meshBuilder.BuildAndBindMesh(
          modelRoot,
          model,
          finToTexCoordAndGltfMaterial);

      var joints = skinNodeAndBones
                   .Select(
                       skinNodeAndBone => skinNodeAndBone.Item1)
                   .ToArray();
      foreach (var gltfMesh in gltfMeshes) {
        // TODO: What causes this to happen???
        if (gltfMesh == null) {
          continue;
        }

        scene.CreateNode()
             .WithSkinnedMesh(gltfMesh,
                              rootNode.WorldMatrix,
                              joints);
      }

      return modelRoot;
    }

    public void Export(IFile outputFile, IModel model) {
      Asserts.True(
          outputFile.Extension.EndsWith(".gltf") ||
          outputFile.Extension.EndsWith(".glb"),
          "Target file is not a GLTF format!");

      this.logger_.BeginScope("Export");

      var modelRoot = this.CreateModelRoot(model);

      var writeSettings = new WriteSettings {
          ImageWriting = this.Embedded
                             ? ResourceWriteMode.Embedded
                             : ResourceWriteMode.SatelliteFile,
      };

      var outputPath = outputFile.FullName;
      this.logger_.LogInformation($"Writing to {outputPath}...");
      modelRoot.Save(outputPath, writeSettings);
    }

    private static MemoryImage
        GetGltfImageFromFinTexture_(ITexture finTexture) {
      using var imageStream = new MemoryStream();
      finTexture.Image.ExportToStream(imageStream, LocalImageFormat.PNG);
      var imageBytes = imageStream.ToArray();
      return new MemoryImage(imageBytes);
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