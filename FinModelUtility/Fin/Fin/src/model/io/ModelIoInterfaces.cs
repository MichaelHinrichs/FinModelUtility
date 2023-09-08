using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.io.bundles;
using fin.language.equations.fixedFunction;
using fin.util.linq;

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
    string Description { get; }
    IReadOnlyList<string> KnownPlatforms { get; }
    IReadOnlyList<string> KnownGames { get; }

    IReadOnlyList<string> MainFileExtensions { get; }
    IReadOnlyList<string> FileExtensions { get; }
  }

  public interface IModelImporterPlugin : IModelPlugin {
    bool SupportsFiles(IEnumerable<IReadOnlySystemFile> files) {
      var fileTypes = files.Select(file => file.FileType).ToArray();

      if (!fileTypes.All(this.FileExtensions.Contains)) {
        return false;
      }

      return fileTypes.Where(this.MainFileExtensions.Contains).Count() == 1;
    }

    IModel ImportModel(IEnumerable<IReadOnlySystemFile> files,
                       out IModelFileBundle outModelFileBundle,
                       float frameRate = 30);
  }

  public interface IModelExporterPlugin : IModelPlugin {
    void ExportModel(IModel model);
  }
}