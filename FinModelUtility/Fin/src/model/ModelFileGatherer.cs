using fin.io.bundles;


namespace fin.model {
  public interface IModelFileBundle : IFileBundle { }

  public interface IModelLoader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel LoadModel(TModelFileBundle modelFileBundle);
  }
}