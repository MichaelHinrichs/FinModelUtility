using System;
using System.Collections.Generic;
using System.Linq;

using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.msg;
using uni.platforms;
using uni.platforms.threeDs;
using uni.util.io;

using zar.api;

namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dExtractor {
    private readonly ILogger logger_ =
        Logging.Create<OcarinaOfTime3dExtractor>();

    public void ExtractAll() {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "ocarina_of_time_3d.cia");

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(ocarinaOfTime3dRom);

      this.ExtractActors_(fileHierarchy);
    }

    private void ExtractActors_(IFileHierarchy fileHierarchy) {
      foreach (var subdir in
          fileHierarchy.Root.TryToGetSubdir("actor").Subdirs) {
        this.ExtractActor_(subdir);
      }
    }

    private void ExtractActor_(IFileHierarchyDirectory actorDirectory) {
      var modelSubdir =
          actorDirectory.Subdirs.SingleOrDefault(dir => dir.Name == "Model");
      if (modelSubdir == null) {
        return;
      }

      var cmbFiles =
          modelSubdir.FilesWithExtension(".cmb").ToArray();
      var csabFiles =
          actorDirectory.Subdirs.SingleOrDefault(dir => dir.Name == "Anim")
                        ?.FilesWithExtension(".csab")
                        .ToArray();

      if (!(cmbFiles.Length == 1 || (csabFiles?.Length ?? 0) == 0)) {
        ;
      }

      this.ExtractModels_(actorDirectory, cmbFiles, csabFiles);
    }

    private void ExtractModels_(
        IFileHierarchyDirectory directory,
        IReadOnlyList<IFileHierarchyFile> cmbFiles,
        IReadOnlyList<IFileHierarchyFile>? csabFiles = null
    ) {
      Asserts.True(cmbFiles.Count > 0);

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(directory);

      var matches = 0;
      var existingModelFiles =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension == ".fbx" ||
                                        file.Extension == ".glb")
                         .ToArray();

      foreach (var cmbFile in cmbFiles) {
        if (existingModelFiles.Any(
            existingModelFile => {
              var existingName = existingModelFile.NameWithoutExtension;
              var cmbName = cmbFile.NameWithoutExtension;

              return cmbName == existingName ||
                     cmbName + "_gltf" == existingName;
            })) {
          ++matches;
        }
      }

      /*if (matches == cmbFiles.Count) {
        this.logger_.LogInformation(
            $"Model(s) already processed from {directory.LocalPath}");
        return;
      }*/

      csabFiles ??= new List<IFileHierarchyFile>();

      if (cmbFiles.Count == 1) {
        MessageUtil.LogExtracting(this.logger_, cmbFiles[0]);
      } else {
        MessageUtil.LogExtracting(this.logger_, directory, cmbFiles);
      }

      try {
        new ManualZar2FbxApi().Run(outputDirectory,
                                   cmbFiles.Select(file => file.Impl)
                                           .ToArray(),
                                   csabFiles.Select(file => file.Impl)
                                            .ToArray());
      } catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }
      this.logger_.LogInformation(" ");
    }
  }
}