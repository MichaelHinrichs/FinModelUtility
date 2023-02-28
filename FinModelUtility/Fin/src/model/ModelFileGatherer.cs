using System.Threading.Tasks;

using fin.io.bundles;


namespace fin.model {
  public interface IModelFileBundle : IFileBundle {
    /// <summary>
    ///   Whether to use a low-level exporter when exporting. This supports
    ///   less features at the moment, but is required for exporting huge
    ///   models without running into out of memory exceptions.
    /// </summary>
    bool UseLowLevelExporter => false;

    bool ForceGarbageCollection => false;
  }

  public interface IModelLoader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel LoadModel(TModelFileBundle modelFileBundle);
  }

  public interface IAsyncModelLoader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    Task<IModel> LoadModelAsync(TModelFileBundle modelFileBundle);
  }
}