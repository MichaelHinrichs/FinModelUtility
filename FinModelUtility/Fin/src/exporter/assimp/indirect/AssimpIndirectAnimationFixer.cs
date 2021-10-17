using Assimp;
using fin.model;
using System.Collections.Generic;

namespace fin.exporter.assimp.indirect {

  public class AssimpIndirectAnimationFixer {
    public void Fix(IModel model, Scene sc) {
      var finAnimations = model.AnimationManager.Animations;
      var assAnimations = sc.Animations;
      for (var a = 0; a < assAnimations.Count; ++a) {
        var assAnimation = assAnimations[a];
        var finAnimation = finAnimations[a];

        // Animations are SUPER slow, we need to speed them way up!
        {
          // Not entirely sure why this is right...
          var animationSpeedup = .001;

          // TODO: Include tangents from the animation file.
          foreach (var channel in assAnimation.NodeAnimationChannels) {
            this.ScaleKeyTimes_(channel.PositionKeys, animationSpeedup);
            this.ScaleKeyTimes_(channel.ScalingKeys, animationSpeedup);
            this.ScaleKeyTimes_(channel.RotationKeys, animationSpeedup);
          }
        }

        var assFps = assAnimation.TicksPerSecond;
        var finFps = finAnimation.FrameRate;

        assAnimation.TicksPerSecond = finFps;
        assAnimation.DurationInTicks *= finFps / assFps;

        // TODO: Include animation looping behavior here.
      }
    }

    private void ScaleKeyTimes_(List<VectorKey> keys, double scale) {
      for (var i = 0; i < keys.Count; ++i) {
        var key = keys[i];
        key.Time *= scale;
        keys[i] = key;
      }
    }

    private void ScaleKeyTimes_(List<QuaternionKey> keys, double scale) {
      for (var i = 0; i < keys.Count; ++i) {
        var key = keys[i];
        key.Time *= scale;
        keys[i] = key;
      }
    }
  }
}