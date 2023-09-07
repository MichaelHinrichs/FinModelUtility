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

    public IModel ImportModel(
        IEnumerable<IReadOnlySystemFile> files,
        float frameRate = 30) {
      var filesArray = files.ToArray();
      var bcxFiles = filesArray.Where(file => file.FileType is ".bca" or ".bck")
                               .ToArray();
      var bmdFile = filesArray.Single(file => file.FileType == ".bmd");
      var btiFiles =
          filesArray.Where(file => file.FileType == ".bti").ToArray();

      var bmdBundle = new BmdModelFileBundle {
          GameName = "",
          BmdFile = bmdFile,
          BcxFiles = bcxFiles,
          BtiFiles = btiFiles,
          FrameRate = frameRate,
      };

      var bmdImporter = new BmdModelImporter();
      return bmdImporter.ImportModel(bmdBundle);
    }
  }
}