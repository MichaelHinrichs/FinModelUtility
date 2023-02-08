using fin.io;
using fin.model;


namespace xmod.api {
  public class PedModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.PedFile;
    public required IFileHierarchyFile PedFile { get; init; }
  }

  public class PedModelLoader : IModelLoader<PedModelFileBundle> {
    public IModel LoadModel(PedModelFileBundle modelFileBundle) {
      throw new NotImplementedException();
    }
  }
}