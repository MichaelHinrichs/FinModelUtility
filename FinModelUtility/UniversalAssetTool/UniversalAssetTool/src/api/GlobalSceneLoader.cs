using fin.model;
using fin.scene;

using games.pikmin2.api;

using hw.api;

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
          VisSceneFileBundle visSceneFileBundle
              => new VisSceneImporter().Import(visSceneFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(sceneFileBundle))
      };
  }
}