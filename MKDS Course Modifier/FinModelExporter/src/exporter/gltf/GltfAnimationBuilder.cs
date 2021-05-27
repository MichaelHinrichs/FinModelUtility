using System.Collections.Generic;
using System.Numerics;

using fin.math;
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

        var fps = animation.Fps;

        // Writes translation/rotation/scale for each joint.
        var translationKeyframes = new Dictionary<float, Vector3>();
        var rotationKeyframes = new Dictionary<float, Quaternion>();
        var scaleKeyframes = new Dictionary<float, Vector3>();
        foreach (var (node, bone) in skinNodesAndBones) {
          if (!animation.BoneTracks.TryGetValue(bone, out var boneTracks)) {
            continue;
          }

          translationKeyframes.Clear();
          rotationKeyframes.Clear();
          scaleKeyframes.Clear();

          foreach (var positionKeyframe in boneTracks.Positions.Keyframes) {
            var position = positionKeyframe.Value;
            translationKeyframes[positionKeyframe.Frame / fps] =
                new Vector3(position.X, position.Y, position.Z);
          }

          foreach (var rotationKeyframe in boneTracks.Rotations.Keyframes) {
            var rotation = rotationKeyframe.Value;
            rotationKeyframes[rotationKeyframe.Frame / fps] =
                QuaternionUtil.Create(rotation);
          }

          foreach (var scaleKeyframe in boneTracks.Scales.Keyframes) {
            var scale = scaleKeyframe.Value;
            scaleKeyframes[scaleKeyframe.Frame / fps] =
                new Vector3(scale.X, scale.Y, scale.Z);
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