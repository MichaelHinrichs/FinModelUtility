using System.Collections.Generic;
using System.Linq;

using fin.io;
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


  public interface IModelPlugin {
    string DisplayName { get; }

    IReadOnlyList<string> MainFileExtensions { get; }
    IReadOnlyList<string> FileExtensions { get; }
  }

  public interface IModelImporterPlugin : IModelPlugin {
    bool SupportsFiles(IEnumerable<IReadOnlySystemFile> files)
      => files.Select(file => file.FileType).All(FileExtensions.Contains);

    bool TryToImportModels(IEnumerable<IReadOnlySystemFile> files,
                           out IModel[] models);
  }

  public interface IModelExporterPlugin : IModelPlugin {
    void ExportModel(IModel model);
  }
}