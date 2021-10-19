using System;
using System.Collections.Generic;
using System.Linq;

using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.msg;
using uni.platforms;
using uni.platforms.gcn;
using uni.util.io;

namespace uni.games.pikmin_2 {
  public class Pikmin2Extractor {
    private readonly ILogger logger_ = Logging.Create<Pikmin2Extractor>();

    public void ExtractAll() {
      var pikmin2Rom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "pikmin_2.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard()
                                             .PruneRarcDumpNames("arc", "data");
      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              pikmin2Rom);

      this.ExtractPikminAndCaptainModels_(fileHierarchy);
      this.ExtractAllFromSeparateDirectories_(fileHierarchy);
      this.ExtractAllFromMergedDirectories_(fileHierarchy);
      this.ExtractLeafBudFlower_(fileHierarchy);
      this.ExtractAllTreasures_(fileHierarchy);
    }

    /// <summary>
    ///   Gets from separate model/animation szs (e.g. Enemies)
    /// </summary>
    private void ExtractAllFromSeparateDirectories_(
        IFileHierarchy fileHierarchy) {
      foreach (var subdir in fileHierarchy) {
        var modelSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "model");
        var animSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "anim");

        if (modelSubdir != null && animSubdir != null) {
          var bmdFiles = modelSubdir.Files;
          var bcxFiles = animSubdir.Impl.GetExistingFiles().ToList();
          var btiFiles = Files.GetFilesWithExtension(subdir.Impl, ".bti", true)
                              .ToList();

          this.ExtractModels_(subdir, bmdFiles, bcxFiles, btiFiles, false);
        }
      }
    }

    /// <summary>
    ///   Gets from model/animations in same szs (e.g. user\Kando)
    /// </summary>
    private void ExtractAllFromMergedDirectories_(
        IFileHierarchy fileHierarchy) {
      foreach (var subdir in fileHierarchy) {
        var arcSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "arc");

        if (arcSubdir != null) {
          this.ExtractModelsInDirectoryAutomatically_(arcSubdir);
        }
      }
    }

    private void ExtractPikminAndCaptainModels_(IFileHierarchy fileHierarchy) {
      var pikminAndCaptainBaseDirectory =
          fileHierarchy.Root.TryToGetSubdir(
              @"user\Kando\piki\pikis_designer");

      var bcxFiles =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("motion")
                                       .Impl.GetExistingFiles()
                                       .ToList();

      var captainSubdir =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("orima_model");
      this.ExtractModels_(captainSubdir,
                          captainSubdir.Files,
                          bcxFiles,
                          null,
                          true);

      var pikminSubdir =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("piki_model");
      this.ExtractModels_(pikminSubdir,
                          pikminSubdir.Files,
                          bcxFiles,
                          null,
                          true);
    }

    private void ExtractAllTreasures_(IFileHierarchy fileHierarchy) {
      var treasureBaseDirectory =
          fileHierarchy.Root.TryToGetSubdir(@"user\Abe\Pellet");


      foreach (var locale in treasureBaseDirectory.Subdirs) {
        foreach (var treasure in locale.Subdirs) {
          var bmdFiles = treasure.Files.Where(file => file.Extension == ".bmd")
                                 .ToArray();
          if (bmdFiles.Length > 0) {
            var bcxFiles =
                treasure.Files
                        .Where(file => file.Extension == ".bca" ||
                                       file.Extension == ".bck")
                        .Select(file => file.Impl)
                        .ToList();
            this.ExtractModels_(treasure, bmdFiles, bcxFiles);
          }
        }
      }
    }

    private void ExtractLeafBudFlower_(IFileHierarchy fileHierarchy) {
      var leafBudFlowerDirectory =
          fileHierarchy.Root.TryToGetSubdir(
              @"user\Kando\piki\pikis_designer\happa_model");
      this.ExtractModelsInDirectoryAutomatically_(leafBudFlowerDirectory);
    }

    private void ExtractModelsInDirectoryAutomatically_(
        IFileHierarchyDirectory directory) {
      var bmdFiles = directory.FilesWithExtension(".bmd").ToArray();
      if (bmdFiles.Length > 0) {
        var bcxFiles = directory.FilesWithExtensions(".bca", ".bck")
                                .Select(file => file.Impl)
                                .ToList();
        var btiFiles =
            Files.GetFilesWithExtension(directory.Impl, ".bti");
        this.ExtractModels_(directory, bmdFiles, bcxFiles, btiFiles, false);
      }
    }

    private void ExtractModels_(
        IFileHierarchyDirectory directory,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFile>? bcxFiles = null,
        IReadOnlyList<IFile>? btiFiles = null,
        bool allowMultipleAnimatedModels = false
    ) {
      Asserts.True(bmdFiles.Count > 0);

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(directory);

      var matches = 0;
      var existingModelFiles =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension == ".fbx" ||
                                        file.Extension == ".glb")
                         .ToArray();

      foreach (var bmdFile in bmdFiles) {
        if (existingModelFiles.Any(
            existingModelFile => {
              var existingName = existingModelFile.Name.Substring(
                  0,
                  existingModelFile.Name.Length -
                  existingModelFile.Extension.Length);
              var bmdName =
                  bmdFile.Name.Substring(0,
                                         bmdFile.Name.Length - ".mod".Length);

              return bmdName == existingName ||
                     bmdName + "_gltf" == existingName;
            })) {
          ++matches;
        }
      }

      if (matches == bmdFiles.Count) {
        this.logger_.LogInformation(
            $"Model(s) already processed from {directory.LocalPath}");
        return;
      }

      bcxFiles ??= new List<IFile>();
      btiFiles ??= new List<IFile>();

      MessageUtil.LogExtracting(this.logger_, directory, bmdFiles);

      try {
        new ManualBmd2FbxApi().Process(outputDirectory,
                                       bmdFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       bcxFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       btiFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       !allowMultipleAnimatedModels);
      } catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }
      this.logger_.LogInformation(" ");
    }
  }
}