using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.model;
using fin.model.io;

namespace j3d.api {
  public class BmdModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Bmd";

    public string Description
      => "Nintendo's JStudio model format.";

    public IReadOnlyList<string> KnownPlatforms { get; } =
      new[] { "GameCube", "Wii" };

    public IReadOnlyList<string> KnownGames { get; } = new[] {
        "Mario Kart: Double Dash", "Pikmin 2", "Super Mario Sunshine"
    };

    public IReadOnlyList<string> MainFileExtensions { get; } = new[] { ".bmd" };

    public IReadOnlyList<string> FileExtensions { get; } =
      new[] { ".bca", ".bck", ".bmd", ".bti" };

    public bool TryToImportModels(
        IEnumerable<IReadOnlySystemFile> files,
        out IModel[] outModels,
        float frameRate = 30) {
      outModels = default;

      var filesArray = files.ToArray();
      var bcxFiles = filesArray.Where(file => file.FileType is ".bca" or ".bck")
                               .ToArray();
      var bmdFiles =
          filesArray.Where(file => file.FileType == ".bmd").ToArray();
      var btiFiles =
          filesArray.Where(file => file.FileType == ".bti").ToArray();

      var bmdBundles = bmdFiles.Select(bmdFiles => new BmdModelFileBundle {
                                   GameName = "",
                                   BmdFile = bmdFiles,
                                   BcxFiles = bcxFiles,
                                   BtiFiles = btiFiles,
                                   FrameRate = frameRate,
                               })
                               .ToArray();
      if (bmdBundles.Length == 0) {
        return false;
      }

      try {
        var bmdImporter = new BmdModelImporter();
        outModels = bmdBundles.Select(bmdImporter.ImportModel).ToArray();
        return true;
      } catch {
        return false;
      }
    }
  }
}