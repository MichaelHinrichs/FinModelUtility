using fin.model;
using fin.model.io.importer;

namespace modl.api {
  public class BattalionWarsModelImporter 
      : IModelImporter<IBattalionWarsModelFileBundle> {
    public IModel ImportModel(IBattalionWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          ModlModelFileBundle modlFileBundle => new ModlModelImporter()
              .ImportModelAsync(modlFileBundle).Result,
          OutModelFileBundle outFileBundle => new OutModelImporter().ImportModel(
              outFileBundle),
          _ => throw new ArgumentOutOfRangeException(
                   nameof(modelFileBundle), modelFileBundle, null)
      };
  }
}