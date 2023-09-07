using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.model;
using fin.model.io;

namespace cmb.api {
  public class CmbModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Cmb";

    public string Description => "Grezzo's models format";

    public IReadOnlyList<string> KnownPlatforms { get; } =
      new[] { "3DS" };

    public IReadOnlyList<string> KnownGames { get; } = new[] {
        "Ever Oasis",
        "Luigi's Mansion 3D",
        "Majora's Mask 3D",
        "Ocarina of Time 3D"
    };


    public IReadOnlyList<string> MainFileExtensions { get; } = new[] { ".cmb" };

    public IReadOnlyList<string> FileExtensions { get; } =
      new[] { ".cmb", ".csab", ".ctxb", ".shpa" };

    public bool TryToImportModels(
        IEnumerable<IReadOnlySystemFile> files,
        out IModel[] outModels,
        float frameRate = 30) {
      outModels = default;

      var filesArray = files.ToArray();
      var csabFiles = filesArray.Where(file => file.FileType is ".csab")
                               .ToArray();
      var cmbFiles =
          filesArray.Where(file => file.FileType == ".cmb").ToArray();
      var ctxbFiles =
          filesArray.Where(file => file.FileType == ".ctxb").ToArray();
      var shpaFiles =
          filesArray.Where(file => file.FileType == ".shpa").ToArray();

      var cmbBundles = cmbFiles
                       .Select(cmbFile => new CmbModelFileBundle(
                                   "",
                                   cmbFile,
                                   csabFiles,
                                   ctxbFiles,
                                   shpaFiles))
                       .ToArray();
      if (cmbBundles.Length == 0) {
        return false;
      }

      try {
        var cmbImporter = new CmbModelImporter();
        outModels = cmbBundles.Select(cmbImporter.ImportModel).ToArray();
        return true;
      } catch {
        return false;
      }
    }
  }
}