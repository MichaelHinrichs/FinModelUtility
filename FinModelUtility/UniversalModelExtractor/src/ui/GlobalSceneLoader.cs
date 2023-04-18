using fin.scene;
using modl.api;
using SuperMario64.api;


namespace uni.ui {
  public class GlobalSceneLoader : ISceneLoader<ISceneFileBundle> {
    public IScene LoadScene(ISceneFileBundle sceneFileBundle)
      => sceneFileBundle switch {
          BwSceneFileBundle bwSceneFileBundle
              => new BwSceneLoader().LoadScene(bwSceneFileBundle),
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneLoader().LoadScene(sm64LevelSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}