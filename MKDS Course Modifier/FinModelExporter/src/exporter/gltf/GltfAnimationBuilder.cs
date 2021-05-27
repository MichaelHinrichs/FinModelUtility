using System.Collections.Generic;
using System.Numerics;

using fin.model;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  using GltfNode = Node;

  public class GltfAnimationBuilder {
    public void BuildAnimations(
        ModelRoot gltfModel,
        (GltfNode, IBone)[] skinNodesAndBones,
        IReadOnlyList<IAnimation> animations) {
      foreach (var animation in animations) {
        var gltfAnimation = gltfModel.UseAnimation(animation.Name);

        // Writes translation/rotation/scale for each joint.
        var translationKeyframes = new Dictionary<float, Vector3>();
        var rotationKeyframes = new Dictionary<float, Quaternion>();
        var scaleKeyframes = new Dictionary<float, Vector3>();
        foreach (var (node, bone) in skinNodesAndBones) {
          if (!animation.BoneTracks.TryGetValue(bone, out var boneTracks)) {
            continue;
          }

          // TODO: Handle mirrored animations
          for (var f = 0; f < animation.FrameCount; ++f) {
            var time = f / animation.Fps;

            var position = boneTracks.Positions.GetInterpolatedAtFrame(f);
            var rotation = boneTracks.Rotations.GetInterpolatedAtFrame(f);
            var scale = boneTracks.Scales.GetInterpolatedAtFrame(f);

            translationKeyframes[time] =
                new Vector3(position.X, position.Y, position.Z);
            rotationKeyframes[time] = rotation;
            scaleKeyframes[time] = new Vector3(scale.X, scale.Y, scale.Z);
          }

          gltfAnimation.CreateTranslationChannel(
              node,
              translationKeyframes);
          gltfAnimation.CreateRotationChannel(
              node,
              rotationKeyframes);
          gltfAnimation.CreateScaleChannel(
              node,
              scaleKeyframes);
        }
      }
    }
  }
}