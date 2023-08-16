using System.Threading.Tasks;

using fin.io.bundles;
using fin.language.equations.fixedFunction;

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

  public interface IModelParameters {
    ILighting? Lighting { get; }
    IFixedFunctionRegisters? Registers { get; }
  }

  public interface IModelLoader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel LoadModel(TModelFileBundle modelFileBundle);
  }

  public interface IAsyncModelLoader<in TModelFileBundle>
      : IModelLoader<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel IModelLoader<TModelFileBundle>.LoadModel(
        TModelFileBundle modelFileBundle)
      => this.LoadModelAsync(modelFileBundle).Result;

    Task<IModel> LoadModelAsync(TModelFileBundle modelFileBundle);
  }
}