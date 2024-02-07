using fin.model;
using fin.scene;

using games.pikmin2.api;

using modl.api;

using sm64.api;

namespace uni.api {
  public class GlobalSceneImporter : ISceneImporter<ISceneFileBundle> {
    public IScene Import(ISceneFileBundle sceneFileBundle)
      => sceneFileBundle switch {
          BwSceneFileBundle bwSceneFileBundle
              => new BwSceneImporter().Import(bwSceneFileBundle),
          Pikmin2SceneFileBundle pikmin2SceneFileBundle
              => new Pikmin2SceneImporter().Import(pikmin2SceneFileBundle),
          Sm64LevelSceneFileBundle sm64LevelSceneFileBundle
              => new Sm64LevelSceneImporter().Import(
                  sm64LevelSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}