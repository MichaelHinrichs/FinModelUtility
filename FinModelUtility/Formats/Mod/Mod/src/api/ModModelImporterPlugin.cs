﻿using System.Collections.Generic;
using System.Linq;

using fin.io;
using fin.model;
using fin.model.io;

namespace mod.api {
  public class ModModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Mod";

    public string Description => "Pikmin 1 model format.";

    public IReadOnlyList<string> KnownPlatforms { get; } =
      new[] { "GameCube" };

    public IReadOnlyList<string> KnownGames { get; } = new[] { "Pikmin 1", };


    public IReadOnlyList<string> MainFileExtensions { get; } = new[] { ".mod" };

    public IReadOnlyList<string> FileExtensions { get; } =
      new[] { ".anm", ".mod" };

    public IModel Import(
        IEnumerable<IReadOnlySystemFile> files,
        out IModelFileBundle outModelFileBundle,
        float frameRate = 30) {
      var filesArray = files.ToArray();
      var anmFile = filesArray.WithFileType(".anm").SingleOrDefault();
      var modFile = filesArray.WithFileType(".mod").Single();

      var modBundle = new ModModelFileBundle {
          GameName = "", AnmFile = anmFile, ModFile = modFile,
      };
      outModelFileBundle = modBundle;

      var modImporter = new ModModelImporter();
      return modImporter.Import(modBundle);
    }
  }
}