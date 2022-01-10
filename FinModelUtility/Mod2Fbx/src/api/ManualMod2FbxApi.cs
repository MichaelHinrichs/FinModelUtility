using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.exporter.gltf;
using fin.io;
using fin.log;
using fin.util.asserts;

using mod.cli;
using mod.gcn;
using mod.gcn.animation;

namespace mod.api {
  public class ManualMod2FbxApi {
    public void Process(
        IDirectory outputDirectory,
        IList<IFile> modFiles,
        IList<IFile> anmFiles,
        bool wasSourcedAutomatically = false,
        bool skipExportingFbx = false) {
      var logger = Logging.Create<ManualMod2FbxApi>();
      logger.LogInformation("Mod2Fbx attempting to parse:");
      logger.LogInformation(
          $"- {modFiles.Count} model(s):\n" +
          string.Join('\n',
                      modFiles.Select(
                          modFile => "    " + modFile.FullName)));
      logger.LogInformation(
          $"- {anmFiles.Count} animation(s):\n" +
          string.Join('\n',
                      anmFiles.Select(
                          anmFile => "    " + anmFile.FullName)));
      logger.LogInformation(" ");

      var multipleModelsWithAnimationInAutomatic =
          wasSourcedAutomatically && modFiles.Count > 1 && anmFiles.Any();
      if (multipleModelsWithAnimationInAutomatic) {
        logger.LogWarning(
            "While automatically gathering files for a directory, found " +
            "multiple MODs and ANMs. Will be matching MODs/ANMs based on " +
            "their name.");
        logger.LogInformation(" ");
      }

      var nonexistentMods = modFiles.Where(modFile => !modFile.Exists);
      var modsExist = !nonexistentMods.Any();
      if (!modsExist) {
        throw new ArgumentException("Some MODs don't exist: " +
                                    string.Join(' ', nonexistentMods));
      }

      var nonexistentAnms = anmFiles.Where(anmFile => !anmFile.Exists);
      var anmsExist = !nonexistentAnms.Any();
      if (!anmsExist) {
        throw new ArgumentException("Some ANMs don't exist: " +
                                    string.Join(' ', nonexistentAnms));
      }

      List<(IFile, Mod)> filesAndMods;
      try {
        filesAndMods = modFiles.Select(
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
        filesAndAnms = anmFiles.Select(
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

      outputDirectory.Create();

      // TODO: Move to indirect model exporter
      logger.LogInformation("Exporting textures.");
      foreach (var (_, mod) in filesAndMods) {
        for (var i = 0; i < mod.textures.Count; ++i) {
          var texture = mod.textures[i];
          texture.ToBitmap()
                 .Save(Path.Combine(outputDirectory.FullName,
                                    $"{texture.Name}.png"));
        }
      }

      logger.LogInformation(
          "Exporting model" + (filesAndMods.Count != 1 ? "s" : "") + ".");
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

        var model = new ModelConverter().Convert(mod, anm);

        if (!skipExportingFbx) {
          new AssimpIndirectExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName, modFile.Name))
                  .CloneWithExtension(".fbx"),
              model);
        } else {
          new GltfExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName, modFile.Name))
                  .CloneWithExtension(".glb"),
              model);
        }
      }
    }
  }
}