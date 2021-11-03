using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using fin.model;
using fin.util.optional;

using SharpGLTF.Schema2;
using SharpGLTF.Transforms;

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

          var translationDefined = boneTracks.Positions.IsDefined;
          var rotationDefined = boneTracks.Rotations.IsDefined;
          var scaleDefined = boneTracks.Scales.IsDefined;

          // TODO: Pass these in directly, not as lists
          var defaultPosition = Optional.Of(new[] {
              bone.LocalPosition.X,
              bone.LocalPosition.Y,
              bone.LocalPosition.Z,
          });
          var defaultRotation = Optional.Of(new[] {
              bone.LocalRotation?.XRadians ?? 0,
              bone.LocalRotation?.YRadians ?? 0,
              bone.LocalRotation?.ZRadians ?? 0,
          });
          var defaultScale = Optional.Of(new[] {
              bone.LocalScale?.X ?? 1,
              bone.LocalScale?.Y ?? 1,
              bone.LocalScale?.Z ?? 1,
          });

          // TODO: How to get keyframes for sparse tracks?
          for (var i = 0; i < animation.FrameCount; ++i) {
            var time = i / fps;

            if (translationDefined) {
              var position =
                  boneTracks.Positions.GetInterpolatedFrame(
                      i,
                      defaultPosition);
              translationKeyframes[time] =
                  new Vector3(position.X, position.Y, position.Z);
            }

            if (rotationDefined) {
              var rotation =
                  boneTracks.Rotations.GetInterpolatedFrame(i, defaultRotation);
              rotationKeyframes[time] = rotation;
            }

            if (scaleDefined) {
              var scale =
                  boneTracks.Scales.GetInterpolatedFrame(i, defaultScale);
              scaleKeyframes[time] = new Vector3(scale.X, scale.Y, scale.Z);
            }
          }

          if (translationDefined) {
            gltfAnimation.CreateTranslationChannel(
                node,
                translationKeyframes);
          }
          if (rotationDefined) {
            gltfAnimation.CreateRotationChannel(
                node,
                rotationKeyframes);
          }
          if (scaleDefined) {
            gltfAnimation.CreateScaleChannel(
                node,
                scaleKeyframes);
          }
        }
      }
    }
  }
}