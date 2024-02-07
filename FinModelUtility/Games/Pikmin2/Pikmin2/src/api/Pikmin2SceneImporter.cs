using fin.io;
using fin.scene;

using games.pikmin2.route;

using jsystem.api;

namespace games.pikmin2.api {
  public class Pikmin2SceneImporter : ISceneImporter<Pikmin2SceneFileBundle> {
    public IScene Import(Pikmin2SceneFileBundle sceneFileBundle) {
      var scene = new SceneImpl();
      var sceneArea = scene.AddArea();

      var mapObj = sceneArea.AddObject();
      mapObj.AddSceneModel(
          new BmdModelImporter().Import(new BmdModelFileBundle {
              GameName = "pikmin_2", BmdFile = sceneFileBundle.LevelBmd
          }));

      var routeObj = sceneArea.AddObject();

      using var routeReader = sceneFileBundle.RouteTxt.OpenReadAsText();
      var route = new RouteParser().Parse(routeReader);
      routeObj.AddSceneModel(new RouteModelBuilder().BuildModel(route));

      return scene;
    }
  }
}