using fin.scene;

using modl.api;

using sm64.api;

namespace uni.ui {
  public class GlobalSceneReader : ISceneReader<ISceneFileBundle> {
    public IScene ReadScene(ISceneFileBundle sceneFileBundle)
      => sceneFileBundle switch {
          BwSceneFileBundle bwSceneFileBundle
              => new BwSceneReader().ReadScene(bwSceneFileBundle),
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneReader().ReadScene(sm64LevelSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}