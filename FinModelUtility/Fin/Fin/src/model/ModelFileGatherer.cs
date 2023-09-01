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

  public interface IModelReader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel ReadModel(TModelFileBundle modelFileBundle);
  }

  public interface IAsyncModelReader<in TModelFileBundle>
      : IModelReader<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel IModelReader<TModelFileBundle>.ReadModel(
        TModelFileBundle modelFileBundle)
      => this.ReadModelAsync(modelFileBundle).Result;

    Task<IModel> ReadModelAsync(TModelFileBundle modelFileBundle);
  }
}