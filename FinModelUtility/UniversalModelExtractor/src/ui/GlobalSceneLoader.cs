using fin.scene;
using sm64.api;


namespace uni.ui {
  public class GlobalSceneLoader : ISceneLoader<ISceneFileBundle> {
    public IScene LoadScene(ISceneFileBundle sceneFileBundle)
      => sceneFileBundle switch {
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneLoader().LoadScene(sm64LevelSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}