using fin.model;


namespace modl.api {
  public class
      BattalionWarsModelLoader : IModelLoader<IBattalionWarsModelFileBundle> {
    public IModel LoadModel(IBattalionWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          ModlModelFileBundle modlFileBundle => new ModlModelLoader().LoadModel(
              modlFileBundle),
          OutModelFileBundle outFileBundle => new OutModelLoader().LoadModel(
              outFileBundle),
      };
  }
}