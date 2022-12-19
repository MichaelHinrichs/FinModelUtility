using fin.data;
using fin.io;
using fin.model;
using fin.scene;
using Quad64;
using Quad64.src.LevelInfo;
using Quad64.src.Scripts;
using System.Numerics;
using BitLogic = Quad64.BitLogic;


namespace sm64.api {
  public class Sm64LevelSceneFileBundle :
      Sm64LevelFileBundle,
      ISceneFileBundle {
    public Sm64LevelSceneFileBundle(
        IFile sm64Rom,
        LevelId levelId) : base(sm64Rom, levelId) { }
  }

  public class Sm64LevelSceneLoader : ISceneLoader<Sm64LevelSceneFileBundle> {
    // TODO: Load this as a scene instead

    public IScene LoadScene(Sm64LevelSceneFileBundle levelModelFileBundle) {
      var sm64Level = Sm64LevelLoader.LoadLevel(levelModelFileBundle);

      var finScene = new SceneImpl();

      var lazyModelDictionary = new LazyDictionary<ushort, IModel?>(
          sm64ModelId => {
            if (sm64Level.ModelIDs.TryGetValue(sm64ModelId,
                                               out var sm64Model)) {
              return Sm64ModelConverter.ConvertModels(sm64Model.HighestLod);
            }
            return null;
          });

      foreach (var sm64Area in sm64Level.Areas) {
        Sm64LevelSceneLoader.AddAreaToScene_(
            finScene,
            lazyModelDictionary,
            sm64Area);
      }

      return finScene;
    }

    private static void AddAreaToScene_(
        IScene finScene,
        LazyDictionary<ushort, IModel?> lazyModelDictionary,
        Area sm64Area) {
      var finArea = finScene.AddArea();
      AddAreaModelToScene_(finArea, sm64Area);

      var objects =
          sm64Area.Objects.Concat(sm64Area.MacroObjects)
                  .Concat(sm64Area.SpecialObjects)
                  .ToArray();

      foreach (var obj in objects) {
        AddAreaObjectToScene_(finArea, lazyModelDictionary, obj);
      }
    }

    private static void AddAreaModelToScene_(ISceneArea finArea, Area sm64Area)
      => finArea.AddObject()
                .AddSceneModel(
                    Sm64ModelConverter.ConvertModels(
                        sm64Area.AreaModel.HighestLod));

    private static void AddAreaObjectToScene_(
        ISceneArea finArea,
        LazyDictionary<ushort, IModel?> lazyModelDictionary,
        Object3D sm64Object) {
      var finModel = lazyModelDictionary[sm64Object.ModelID];
      if (finModel == null) {
        return;
      }

      var finObject = finArea.AddObject();
      finObject.AddSceneModel(finModel);
      finObject.SetPosition(sm64Object.xPos, sm64Object.yPos, sm64Object.zPos);
      finObject.SetRotationDegrees(sm64Object.xRot, sm64Object.yRot, sm64Object.zRot);

      var scale = 1f;
      var billboard = false;

      var scripts = sm64Object.ParseBehavior();
      foreach (var script in scripts) {
        if (script.Command == BehaviorCommand.SCALE) {
          var rawScale = BitLogic.BytesToInt(script.data, 2, 2);
          scale = rawScale / 100f;
        }

        if (script.Command == BehaviorCommand.billboard) {
          billboard = true;
        }
      }

      finObject.SetScale(scale, scale, scale);
      if (billboard) {
        var rotateYaw =
            Quaternion.CreateFromYawPitchRoll(-MathF.PI / 2, 0, 0);
        finModel.Skeleton.Root.AlwaysFaceTowardsCamera(rotateYaw);
      }
    }
  }
}