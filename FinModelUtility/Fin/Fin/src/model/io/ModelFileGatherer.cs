using fin.io.bundles;
using fin.language.equations.fixedFunction;

namespace fin.model.io {
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
}