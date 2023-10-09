using System.Threading.Tasks;

namespace fin.model.io.importers {

  public interface IModelImporter<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel ImportModel(TModelFileBundle modelFileBundle);
  }

  public interface IAsyncModelImporter<in TModelFileBundle>
      : IModelImporter<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel IModelImporter<TModelFileBundle>.ImportModel(
        TModelFileBundle modelFileBundle)
      => this.ImportModelAsync(modelFileBundle).Result;

    Task<IModel> ImportModelAsync(TModelFileBundle modelFileBundle);
  }
}
