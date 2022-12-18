using fin.io;
using fin.model;


namespace sm64.api {
  public class Sm64LevelModelFileBundle :
      Sm64LevelFileBundle,
      IModelFileBundle {
    public Sm64LevelModelFileBundle(
        IFile sm64Rom,
        LevelId levelId) : base(sm64Rom, levelId) { }
  }

  public class Sm64LevelModelLoader : IModelLoader<Sm64LevelModelFileBundle> {
    public IModel LoadModel(Sm64LevelModelFileBundle levelModelFileBundle) {
      var level = Sm64LevelLoader.LoadLevel(levelModelFileBundle);

      var finModel = Sm64ModelConverter.ConvertModels(
          level.Areas.Select(area => area.AreaModel.HighestLod).ToArray());

      return finModel;
    }
  }
}