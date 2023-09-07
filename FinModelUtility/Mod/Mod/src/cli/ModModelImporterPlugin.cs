using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.model;
using fin.model.io;

using mod.cli;

namespace mod.api {
  public class ModModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Mod";

    public string Description => "Pikmin 1 models format";

    public IReadOnlyList<string> KnownPlatforms { get; } =
      new[] { "GameCube" };

    public IReadOnlyList<string> KnownGames { get; } = new[] { "Pikmin 1", };


    public IReadOnlyList<string> MainFileExtensions { get; } = new[] { ".mod" };

    public IReadOnlyList<string> FileExtensions { get; } =
      new[] { ".anm", ".mod" };

    public bool TryToImportModels(
        IEnumerable<IReadOnlySystemFile> files,
        out IModel[] outModels,
        float frameRate = 30) {
      outModels = default;

      var filesArray = files.ToArray();
      var anmFile =
          filesArray.Where(file => file.FileType == ".anm")
                    .ToArray()
                    .SingleOrDefault();
      var modFiles = filesArray.Where(file => file.FileType is ".mod")
                               .ToArray();

      var modBundles = modFiles
          .Select(modFile => new ModModelFileBundle {
              GameName = "",
              AnmFile = anmFile,
              ModFile = modFile,
          }).ToArray();
      if (modBundles.Length == 0) {
        return false;
      }

      try {
        var modImporter = new ModModelImporter();
        outModels = modBundles.Select(modImporter.ImportModel).ToArray();
        return true;
      } catch {
        return false;
      }
    }
  }
}