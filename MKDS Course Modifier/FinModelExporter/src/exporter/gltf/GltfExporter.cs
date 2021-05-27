using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.log;
using fin.math;
using fin.model;

using MathNet.Numerics.LinearAlgebra;

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

      if (firstAnimation != null) {
        // TODO: Put this somewhere else!
        // We should be able to use the raw bone positions, but this screws up
        // bones with multiple weights for some reason, perhaps because the
        // model is contorted in an unnatural way? Anyway, we NEED to use the
        // first animation instead.
        this.ApplyFirstFrameToSkeleton_(model.Skeleton, firstAnimation);
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

      // Builds mesh.
      var mesh = new GltfMeshBuilder().BuildAndBindMesh(modelRoot, model);
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

    // TODO: Pull this out somewhere else, or make this part of the model creation flow?
    private void ApplyFirstFrameToSkeleton_(
        ISkeleton skeleton,
        IAnimation animation) {
      var boneQueue = new Queue<IBone>();
      boneQueue.Enqueue(skeleton.Root);

      while (boneQueue.Count > 0) {
        var bone = boneQueue.Dequeue();

        animation.BoneTracks.TryGetValue(bone, out var boneTracks);

        var localPosition = boneTracks?.Positions.GetInterpolatedAtFrame(0);
        bone.SetLocalPosition(localPosition?.X ?? 0,
                              localPosition?.Y ?? 0,
                              localPosition?.Z ?? 0);

        var localRotation = boneTracks?.Rotations.GetAtFrame(0);
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
  }
}