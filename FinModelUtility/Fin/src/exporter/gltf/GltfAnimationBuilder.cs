using System.Collections.Generic;
using System.Linq;
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

        var fps = animation.FrameRate;

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

          // TODO: How to get keyframes for sparse tracks?
          for (var i = 0; i < animation.FrameCount; ++i) {
            var time = i / fps;

            var position = boneTracks.Positions.GetInterpolatedFrame(i);
            translationKeyframes[time] =
                new Vector3(position.X, position.Y, position.Z);

            var rotation = boneTracks.Rotations.GetInterpolatedFrame(i);
            rotationKeyframes[time] = rotation;

            var scale = boneTracks.Scales.GetInterpolatedFrame(i);
            scaleKeyframes[time] = new Vector3(scale.X, scale.Y, scale.Z);
          }

          // TODO: Delete this
          if (boneTracks.Positions.GetAxisListAtKeyframe(0)
                        .Any(axis => axis.Pluck(keyframe => keyframe.Tangent)
                                         .HasValue)) {

            var u = boneTracks.Rotations.AxisTracks[0].Keyframes;
            ;
          }

          if (boneTracks.Positions.IsDefined) {
            gltfAnimation.CreateTranslationChannel(
                node,
                translationKeyframes);
          }
          if (boneTracks.Rotations.IsDefined) {
            gltfAnimation.CreateRotationChannel(
                node,
                rotationKeyframes);
          }
          if (boneTracks.Scales.IsDefined) {
            gltfAnimation.CreateScaleChannel(
                node,
                scaleKeyframes);
          }
        }
      }
    }
  }
}