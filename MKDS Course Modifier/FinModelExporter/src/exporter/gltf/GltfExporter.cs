using System.IO;
using System.Linq;

using fin.log;
using fin.model;

using Microsoft.Extensions.Logging;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  public class GltfExporter : IExporter {
    private readonly ILogger logger_ = Logging.Create<GltfExporter>();

    public void Export(
        string outputPath,
        IModel model) {
      this.logger_.BeginScope("Export");

      var modelRoot = ModelRoot.CreateModel();

      var scene = modelRoot.UseScene("default");
      var skin = modelRoot.CreateSkin();

      var animations = model.AnimationManager?.Animations;
      var firstAnimation = (animations?.Count ?? 0) > 0 ? animations[0] : null;

      // Builds skeleton.
      var rootNode = scene.CreateNode();
      var skinNodeAndBones = new GltfSkeletonBuilder().BuildAndBindSkeleton(
          rootNode,
          skin,
          model.Skeleton,
          firstAnimation);

      // Builds animations.
      new GltfAnimationBuilder().BuildAnimations(
          modelRoot,
          skinNodeAndBones,
          model.AnimationManager.Animations);

      // Builds mesh.
      var mesh =
          new GltfMeshBuilder().BuildAndBindMesh(
              modelRoot,
              model,
              firstAnimation);
      scene.CreateNode()
           .WithSkinnedMesh(mesh,
                            rootNode.WorldMatrix,
                            skinNodeAndBones
                                .Select(
                                    skinNodeAndBone => skinNodeAndBone.Item1)
                                .ToArray());

      Directory.CreateDirectory(new FileInfo(outputPath).Directory.FullName);
      var writeSettings = new WriteSettings {
          ImageWriting = ResourceWriteMode.SatelliteFile,
      };

      this.logger_.LogInformation($"Writing to {outputPath}...");
      modelRoot.Save(outputPath, writeSettings);
    }
  }
}