using System.Threading.Tasks;

namespace fin.model.io.importer {

  public interface IModelImporter<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel ImportModel(TModelFileBundle modelFileBundle,
                       IModelParameters? modelParameters = null);
  }

  public interface IAsyncModelImporter<in TModelFileBundle>
      : IModelImporter<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel IModelImporter<TModelFileBundle>.ImportModel(
        TModelFileBundle modelFileBundle,
        IModelParameters? modelParameters)
      => this.ImportModelAsync(modelFileBundle, modelParameters).Result;

    Task<IModel> ImportModelAsync(TModelFileBundle modelFileBundle,
                                  IModelParameters? modelParameters = null);
  }
}
