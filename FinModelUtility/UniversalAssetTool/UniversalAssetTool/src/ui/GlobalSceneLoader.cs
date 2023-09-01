using fin.scene;

using modl.api;

using sm64.api;

namespace uni.ui {
  public class GlobalSceneImporter : ISceneImporter<ISceneFileBundle> {
    public IScene ImportScene(ISceneFileBundle sceneFileBundle)
      => sceneFileBundle switch {
          BwSceneFileBundle bwSceneFileBundle
              => new BwSceneImporter().ImportScene(bwSceneFileBundle),
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneImporter().ImportScene(sm64LevelSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}