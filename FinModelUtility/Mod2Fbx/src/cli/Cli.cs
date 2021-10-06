using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.log;
using fin.util.asserts;

using Microsoft.Extensions.Logging;

using mod.gcn;
using mod.gcn.animation;

namespace mod.cli {
  public class Cli {
    public static int Main(string[] args) {
      Args.PopulateFromArgs(args);

      var logger = Logging.Create<Cli>();
      using var _ = logger.BeginScope("Entry");
      logger.LogInformation(string.Join(" ", args));

      using var _2 = logger.BeginScope("Main");
      logger.LogInformation("Attempting to parse:");
      logger.LogInformation(
          $"- {Args.ModFiles.Count} model(s):\n" +
          string.Join('\n', Args.ModFiles));
      logger.LogInformation(
          $"- {Args.AnmFiles.Count} animation(s):\n" +
          string.Join('\n', Args.AnmFiles));

      var multipleModelsWithAnimationInAutomatic =
          Args.Automatic && Args.ModFiles.Count > 1 && Args.AnmFiles.Any();
      if (multipleModelsWithAnimationInAutomatic) {
        logger.LogWarning(
            "While automatically gathering files for a directory, found " +
            "multiple MODs and ANMs. Will be matching MODs/ANMs based on " +
            "their name.");
      }

      var nonexistentMods = Args.ModFiles.Where(modFile => !modFile.Exists);
      var modsExist = !nonexistentMods.Any();
      if (!modsExist) {
        throw new ArgumentException("Some MODs don't exist: " +
                                    string.Join(' ', nonexistentMods));
      }

      var nonexistentAnms = Args.AnmFiles.Where(anmFile => !anmFile.Exists);
      var anmsExist = !nonexistentAnms.Any();
      if (!anmsExist) {
        throw new ArgumentException("Some ANMs don't exist: " +
                                    string.Join(' ', nonexistentAnms));
      }

      List<(IFile, Mod)> filesAndMods;
      try {
        filesAndMods = Args.ModFiles.Select(
                               modFile => {
                                 var mod = new Mod();
                                 mod.Read(
                                     new EndianBinaryReader(
                                         modFile.OpenRead()));
                                 return (modFile, mod);
                               })
                           .ToList();
      } catch {
        logger.LogError("Failed to load MOD!");
        throw;
      }

      List<(IFile, Anm)> filesAndAnms;
      try {
        filesAndAnms = Args.AnmFiles.Select(
                               anmFile => {
                                 var anm = new Anm();
                                 anm.Read(
                                     new EndianBinaryReader(
                                         anmFile.OpenRead()));
                                 return (anmFile, anm);
                               })
                           .ToList();
      } catch {
        logger.LogError("Failed to load ANM!");
        throw;
      }

      var outputDirectory = Args.OutputDirectory;
      outputDirectory.Create();

      logger.LogInformation("Exporting textures.");
      foreach (var (_, mod) in filesAndMods) {
        for (var i = 0; i < mod.textures.Count; ++i) {
          var texture = mod.textures[i];
          texture.ToBitmap()
                 .Save(Path.Combine(outputDirectory.FullName,
                                    $"{texture.Name}.png"));
        }
      }

      logger.LogInformation("Exporting model.");
      foreach (var (modFile, mod) in filesAndMods) {
        IList<(IFile, Anm)> modFilesAndAnms;
        if (multipleModelsWithAnimationInAutomatic) {
          modFilesAndAnms = filesAndAnms.Where(fileAndAnm => {
                                          var expectedModFile =
                                              fileAndAnm.Item1
                                                  .CloneWithExtension(".mod");
                                          return expectedModFile.FullName ==
                                                 modFile.FullName;
                                        })
                                        .ToList();
        } else {
          modFilesAndAnms = filesAndAnms;
        }

        Asserts.True(modFilesAndAnms.Count <= 1,
                     "Mod2fbx doesn't support multiple animation files yet!");

        Anm? anm = null;
        if (modFilesAndAnms.Count > 0) {
          anm = modFilesAndAnms[0].Item2;
        }

        var model = ModelConverter.Convert(mod, anm);

        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName, modFile.Name))
                .CloneWithExtension(".fbx"),
            model);
      }

      return 0;
    }
  }
}