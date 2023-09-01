using fin.model;

namespace modl.api {
  public class BattalionWarsModelReader 
      : IModelReader<IBattalionWarsModelFileBundle> {
    public IModel ReadModel(IBattalionWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          ModlModelFileBundle modlFileBundle => new ModlModelReader()
              .ReadModelAsync(modlFileBundle).Result,
          OutModelFileBundle outFileBundle => new OutModelReader().ReadModel(
              outFileBundle),
          _ => throw new ArgumentOutOfRangeException(
                   nameof(modelFileBundle), modelFileBundle, null)
      };
  }
}