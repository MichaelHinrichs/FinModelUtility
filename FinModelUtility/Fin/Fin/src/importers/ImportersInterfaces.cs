using fin.io.bundles;

namespace fin.importers {
  public interface IImporter<out T, in TFileBundle>
      where TFileBundle : IFileBundle {
    T Import(TFileBundle fileBundle);
  }


  public interface I3dFileBundle : IFileBundle {
    /// <summary>
    ///   Whether to use a low-level exporter when exporting. This supports
    ///   less features at the moment, but is required for exporting huge
    ///   models without running into out of memory exceptions.
    /// </summary>
    bool UseLowLevelExporter => false;

    bool ForceGarbageCollection => false;
  }

  public interface I3dImporter<out T, in TFileBundle>
      : IImporter<T, TFileBundle>
      where TFileBundle : I3dFileBundle;
}