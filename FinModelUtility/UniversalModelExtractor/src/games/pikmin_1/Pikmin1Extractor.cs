using System;
using System.Linq;

using fin.io;
using fin.log;

using mod.api;

using uni.platforms;
using uni.platforms.gcn;
using uni.platforms.gcn.tools;
using uni.src.msg;

namespace uni.games.pikmin_1 {
  public class Pikmin1Extractor {
    public void ExtractAll() {
      var pikmin1Rom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "pikmin_1.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(pikmin1Rom);

      var logger = Logging.Create<Pikmin1Extractor>();

      fileHierarchy.ForEach(fileHierarchyDirectory => {
        // TODO: Handle special cases:
        // - olimar
        // - pikmin
        // - frog

        var modFiles =
            fileHierarchyDirectory.Files.Where(
                                      file => file.Extension == ".mod")
                                  .ToArray();

        if (modFiles.Length == 0) {
          return;
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
                var existingName = existingModelFile.Name.Substring(
                    0,
                    existingModelFile.Name.Length -
                    existingModelFile.Extension.Length);
                var modName =
                    modFile.Name.Substring(0,
                                           modFile.Name.Length - ".mod".Length);

                return modName == existingName ||
                       modName + "_gltf" == existingName;
              })) {
            ++matches;
          }
        }

        if (matches == modFiles.Length) {
          logger.LogInformation(
              $"Model(s) already processed from {fileHierarchyDirectory.LocalPath}");
          return;
        }

        // TODO: For some reason, the program hangs when trying to export the
        // title screen as FBX.
        var skipExportingFbx = fileHierarchyDirectory.LocalPath ==
                               @"\dataDir\cinemas\titles";

        MessageUtil.LogExtracting(logger, fileHierarchyDirectory, modFiles);

        var anmFiles =
            fileHierarchyDirectory.Files.Where(
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
      });
    }
  }
}