using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.model;
using fin.model.io;

namespace cmb.api {
  public class CmbModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Cmb";

    public string Description => "Grezzo's model format.";

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

    public IModel ImportModel(IEnumerable<IReadOnlySystemFile> files,
                              out IModelFileBundle outModelFileBundle,
                              float frameRate = 30) {
      var filesArray = files.ToArray();
      var csabFiles = filesArray.Where(file => file.FileType is ".csab")
                                .ToArray();
      var cmbFile = filesArray.Single(file => file.FileType == ".cmb");
      var ctxbFiles =
          filesArray.Where(file => file.FileType == ".ctxb").ToArray();
      var shpaFiles =
          filesArray.Where(file => file.FileType == ".shpa").ToArray();

      var cmbBundle = new CmbModelFileBundle(
          "",
          cmbFile,
          csabFiles,
          ctxbFiles,
          shpaFiles);
      outModelFileBundle = cmbBundle;

      var cmbImporter = new CmbModelImporter();
      return cmbImporter.ImportModel(cmbBundle);
    }
  }
}