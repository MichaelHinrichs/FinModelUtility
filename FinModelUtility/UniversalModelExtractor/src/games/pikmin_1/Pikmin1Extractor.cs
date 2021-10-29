using System;
using System.Linq;

using fin.log;

using mod.api;

using uni.msg;
using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.pikmin_1 {
  public class Pikmin1Extractor {
    public void ExtractAll() {
      var pikmin1Rom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "pikmin_1.gcm");

      var options = GcnFileHierarchyExtractor.Options.Empty();
      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(options, pikmin1Rom);

      var logger = Logging.Create<Pikmin1Extractor>();

      foreach (var subdir in fileHierarchy) {
        // TODO: Handle special cases:
        // - olimar
        // - pikmin
        // - frog

        var modFiles = subdir.FilesWithExtension(".mod").ToArray();

        if (modFiles.Length == 0) {
          continue;
        }

        var outputDirectory =
            GameFileHierarchyUtil.GetOutputDirectoryForFile(modFiles[0]);

        var matches = 0;
        var existingModelFiles =
            outputDirectory.GetExistingFiles()
                           .Where(file => file.Extension == ".fbx" ||
                                          file.Extension == ".glb")
                           .ToArray();

        foreach (var modFile in modFiles) {
          if (existingModelFiles.Any(
              existingModelFile => {
                var existingName = existingModelFile.NameWithoutExtension;
                var modName = modFile.NameWithoutExtension;

                return modName == existingName ||
                       modName + "_gltf" == existingName;
              })) {
            ++matches;
          }
        }

        if (matches == modFiles.Length) {
          logger.LogInformation(
              $"Model(s) already processed from {subdir.LocalPath}");
          continue;
        }

        // TODO: For some reason, the program hangs when trying to export the
        // title screen as FBX.
        var skipExportingFbx = subdir.LocalPath ==
                               @"\dataDir\cinemas\titles";

        MessageUtil.LogExtracting(logger, subdir, modFiles);

        var anmFiles =
            subdir.Files.Where(
                      file => file.Extension == ".anm")
                  .ToArray();

        try {
          new ManualMod2FbxApi().Process(outputDirectory,
                                         modFiles.Select(file => file.Impl)
                                                 .ToArray(),
                                         anmFiles.Select(file => file.Impl)
                                                 .ToArray(),
                                         true,
                                         skipExportingFbx);
        } catch (Exception e) {
          logger.LogError(e.ToString());
        }
        logger.LogInformation(" ");
      }
    }
  }
}