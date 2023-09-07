using fin.config;
using fin.io.bundles;
using fin.model;
using fin.model.util;
using fin.scene;

using uni.config;
using uni.platforms;

namespace uni.model {
  public interface IScaleSource {
    float GetScale(IScene scene, IAnnotatedFileBundle fileBundle);
    float GetScale(IModel model, IAnnotatedFileBundle fileBundle);
  }

  public class ScaleSource : IScaleSource {
    private readonly IScaleSource impl_;

    public ScaleSource(ScaleSourceType type) {
      this.impl_ = type switch {
          ScaleSourceType.NONE => new NullScaleSource(),
          ScaleSourceType.MIN_MAX_BOUNDS => new MinMaxBoundsScaleSource(),
          ScaleSourceType.GAME_CONFIG => new GameConfigScaleSource(),
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
    }

    public float GetScale(IScene scene, IAnnotatedFileBundle fileBundle)
      => this.impl_.GetScale(scene, fileBundle);

    public float GetScale(IModel model, IAnnotatedFileBundle fileBundle)
      => this.impl_.GetScale(model, fileBundle);
  }

  public class NullScaleSource : IScaleSource {
    public float GetScale(IScene _1, IAnnotatedFileBundle _2) => 1;
    public float GetScale(IModel _1, IAnnotatedFileBundle _2) => 1;
  }

  public class MinMaxBoundsScaleSource : IScaleSource {
    public float GetScale(IScene scene, IAnnotatedFileBundle _)
      => new SceneMinMaxBoundsScaleCalculator().CalculateScale(scene);

    public float GetScale(IModel model, IAnnotatedFileBundle _)
      => new ModelMinMaxBoundsScaleCalculator().CalculateScale(model);
  }

  public class GameConfigScaleSource : IScaleSource {
    public float GetScale(IScene scene, IAnnotatedFileBundle fileBundle)
      => this.TryToGetScaleFromGameConfig_(fileBundle, out float scale)
          ? scale
          : 1;

    public float GetScale(IModel model, IAnnotatedFileBundle fileBundle)
      => this.TryToGetScaleFromGameConfig_(fileBundle, out float scale)
          ? scale
          : 1;

    private bool TryToGetScaleFromGameConfig_(IAnnotatedFileBundle fileBundle,
                                              out float scale) {
      var gameName = fileBundle.FileBundle.GameName;
      if (gameName != null &&
          DirectoryConstants.GAME_CONFIG_DIRECTORY.TryToGetExistingFile(
              $"{gameName}.json",
              out var gameConfigFile)) {
        var gameConfig = gameConfigFile.Deserialize<GameConfig>();
        scale = gameConfig.Scale;
        return true;
      }

      scale = 1;
      return false;
    }
  }
}