using System.IO;

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

      var rootNode = scene.CreateNode();
      new GltfSkeletonBuilder().BuildAndBindSkeleton(rootNode,
        skin,
        model.Skeleton,
        firstAnimation);

      // Gathers up vertex builders.
      /*var mesh = GltfExporterOld.WriteMesh_(jointNodes, model, bmd, pathsAndBtis);
      scene.CreateNode()
           .WithSkinnedMesh(mesh, rootNode.WorldMatrix, jointNodes.ToArray());*/

      Directory.CreateDirectory(new FileInfo(outputPath).Directory.FullName);
      var writeSettings = new WriteSettings {
          ImageWriting = ResourceWriteMode.SatelliteFile,
      };

      this.logger_.LogInformation($"Writing to {outputPath}...");
      modelRoot.Save(outputPath, writeSettings);
    }
  }
}