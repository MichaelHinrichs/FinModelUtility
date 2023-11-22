using fin.model;
using fin.scene;

using games.pikmin2.api;

using modl.api;

using sm64.api;

namespace uni.api {
  public class GlobalSceneImporter : ISceneImporter<ISceneFileBundle> {
    public IScene ImportScene(ISceneFileBundle sceneFileBundle,
                              out ILighting? lighting)
      => sceneFileBundle switch {
          BwSceneFileBundle bwSceneFileBundle
              => new BwSceneImporter().ImportScene(
                  bwSceneFileBundle,
                  out lighting),
          Pikmin2SceneFileBundle pikmin2SceneFileBundle
              => new Pikmin2SceneImporter().ImportScene(
                  pikmin2SceneFileBundle,
                  out lighting),
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneImporter().ImportScene(
                  sm64LevelSceneFileBundle,
                  out lighting),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}