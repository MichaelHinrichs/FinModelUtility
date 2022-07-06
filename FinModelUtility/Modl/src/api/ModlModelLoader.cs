using fin.io;
using fin.model;


namespace modl.api {
  public class ModlModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile { get; }
  }

  public class ModlModelLoader : IModelLoader<ModlModelFileBundle> {
    public IModel LoadModel(ModlModelFileBundle modelFileBundle) {
      throw new NotImplementedException();
    }
  }
}
