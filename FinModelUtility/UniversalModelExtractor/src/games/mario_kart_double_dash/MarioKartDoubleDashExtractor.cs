using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.msg;
using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.mario_kart_double_dash {
  public class MarioKartDoubleDashExtractor {
    private readonly ILogger logger_ =
        Logging.Create<MarioKartDoubleDashExtractor>();

    public void ExtractAll() {
      var marioKartDoubleDashRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "mario_kart_double_dash.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard()
                                             .UseRarcDumpForExtensions(".arc");
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, marioKartDoubleDashRom);

      this.ExtractDrivers_(fileHierarchy);
      this.ExtractKarts_(fileHierarchy);
      this.ExtractCourses_(fileHierarchy);
      // TODO: Extract "enemies"
      // TODO: Extract "objects"
    }

    private void ExtractKarts_(IFileHierarchy fileHierarchy) {
      var kartSubdir = fileHierarchy.Root.TryToGetSubdir(@"MRAM\kart");

      foreach (var subdir in kartSubdir.Subdirs) {
        var bmdFiles = subdir.FilesWithExtension(".bmd")
                             .ToArray();
        this.ExtractModels_(subdir, bmdFiles);
      }
    }

    private void ExtractDrivers_(IFileHierarchy fileHierarchy) {
      var mramSubdir = fileHierarchy.Root.TryToGetSubdir(@"MRAM\driver");

      {
        var plumberNames = new[] {"mario", "luigi",};
        var plumberSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => plumberNames.Contains(subdir.Name));
        var plumberCommon = mramSubdir.TryToGetSubdir("cmn_hige");
        foreach (var plumberSubdir in plumberSubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              plumberSubdir,
              plumberCommon);
        }
      }

      {
        var babyNames = new[] {
            "babymario", "babyluigi",
            // Should the toads actually be included here?
            "kinipio", "kinopico"
        };
        var babySubdirs =
            mramSubdir.Subdirs.Where(
                subdir => babyNames.Contains(subdir.Name));
        var babyCommon = mramSubdir.TryToGetSubdir("cmn_baby");
        foreach (var babySubdir in babySubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              babySubdir,
              babyCommon);
        }
      }

      {
        var princessNames = new[] {"daisy", "peach"};
        var princessSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => princessNames.Contains(subdir.Name));
        var princessCommon = mramSubdir.TryToGetSubdir("cmn_hime");
        foreach (var princessSubdir in princessSubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              princessSubdir,
              princessCommon);
        }
      }

      {
        var lizardNames = new[] {"catherine", "yoshi"};
        var lizardSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => lizardNames.Contains(subdir.Name));
        var lizardCommon = mramSubdir.TryToGetSubdir("cmn_liz");
        foreach (var lizardSubdir in lizardSubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              lizardSubdir,
              lizardCommon);
        }
      }

      // TODO: Where are toad's animations?

      {
        var koopaNames = new[] {"patapata", "nokonoko"};
        var koopaSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => koopaNames.Contains(subdir.Name));
        var koopaCommon = mramSubdir.TryToGetSubdir("cmn_zako");
        foreach (var koopaSubdir in koopaSubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              koopaSubdir,
              koopaCommon);
        }
      }

      {
        var standaloneNames = new[] {
            "bosspakkun",
            "dk",
            "dkjr",
            "kingteresa",
            "koopa",
            "koopajr",
            "waluigi",
            "wario",
        };
        var standaloneSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => standaloneNames.Contains(subdir.Name));
        foreach (var standaloneSubdir in standaloneSubdirs) {
          this.ExtractFromDriverDirectory_(standaloneSubdir);
        }
      }
    }

    private void ExtractFromDriverDirectory_(
        IFileHierarchyDirectory directory) {
      var bmdFiles = directory.FilesWithExtension(".bmd")
                              .ToArray();
      var bcxFiles = directory.FilesWithExtensions(".bca", ".bck")
                              .Select(file => file.Impl)
                              .ToArray();

      var driverBmdFiles = bmdFiles
                           .Where(file => file.Name.StartsWith("driver"))
                           .ToArray();
      var driverBcxFiles =
          bcxFiles.Where(file => file.Name.StartsWith("b_") ||
                                 file.Name.StartsWith("c_") ||
                                 file.Name.StartsWith("all"))
                  .ToArray();
      this.ExtractModels_(directory,
                          driverBmdFiles,
                          driverBcxFiles,
                          null,
                          true);

      var otherBmdFiles = bmdFiles.Where(file => !driverBmdFiles.Contains(file))
                                  .ToArray();
      if (otherBmdFiles.Length > 0) {
        var otherBcxFiles =
            bcxFiles.Where(file => !driverBcxFiles.Contains(file))
                    .ToArray();
        this.ExtractModels_(directory,
                            otherBmdFiles,
                            otherBcxFiles,
                            null,
                            true);
      }
    }

    private void ExtractFromSeparateDriverDirectories_(
        IFileHierarchyDirectory directory,
        IFileHierarchyDirectory common) {
      Asserts.Nonnull(common);

      var bmdFiles = directory.FilesWithExtension(".bmd")
                              .ToArray();
      var commonBcxFiles = common.FilesWithExtensions(".bca", ".bck")
                                 .Select(file => file.Impl)
                                 .ToArray();
      var localBcxFiles = directory.FilesWithExtensions(".bca", ".bck")
                                   .Select(file => file.Impl)
                                   .ToArray();

      this.ExtractModels_(directory,
                          bmdFiles,
                          commonBcxFiles.Concat(localBcxFiles).ToArray(),
                          null,
                          true);
    }

    private void ExtractCourses_(IFileHierarchy fileHierarchy) {
      var courseSubdir = fileHierarchy.Root.TryToGetSubdir("Course");

      foreach (var subdir in courseSubdir.Subdirs) {
        var bmdFiles = subdir.FilesWithExtension(".bmd")
                             .ToArray();
        if (bmdFiles.Length == 0) {
          continue;
        }

        var btiFiles = subdir.FilesWithExtension(".bti")
                             .Select(file => file.Impl)
                             .ToArray();
        this.ExtractModels_(subdir,
                            bmdFiles,
                            null,
                            btiFiles,
                            true);

        var objectsSubdir = subdir.TryToGetSubdir("objects");
        this.ExtractModelsAndAnimationsFromSceneObject_(objectsSubdir);
      }
    }

    private void ExtractModelsAndAnimationsFromSceneObject_(
        IFileHierarchyDirectory directory) {
      var bmdFiles = directory.Files.Where(
                                  file => file.Extension == ".bmd")
                              .OrderByDescending(file => file.Name.Length)
                              .ToArray();
      var allBcxFiles = directory
                        .Files.Where(
                            file => file.Extension == ".bck" ||
                                    file.Extension == ".bca")
                        .Select(file => file.Impl)
                        .ToArray();
      var btiFiles = directory.FilesWithExtension(".bti")
                              .Select(file => file.Impl)
                              .ToArray();

      // If there is only one model or 0 animations, it's easy to tell which
      // animations go with which model.
      if (bmdFiles.Length == 1 || allBcxFiles.Length == 0) {
        foreach (var bmdFile in bmdFiles) {
          this.ExtractModels_(directory,
                              new[] {
                                  bmdFile
                              },
                              allBcxFiles,
                              btiFiles);
        }
        return;
      }

      var unclaimedBcxFiles = allBcxFiles.ToHashSet();
      var bmdAndBcxFiles = new Dictionary<IFileHierarchyFile, IFile[]>();
      foreach (var bmdFile in bmdFiles) {
        var prefix = bmdFile.Name;
        prefix = prefix.Substring(0, prefix.Length - ".bmd".Length);

        // Blegh. These special cases are gross.
        {
          var modelIndex = prefix.IndexOf("_model");
          if (modelIndex != -1) {
            prefix = prefix.Substring(0, modelIndex);
          }

          var babyIndex = prefix.IndexOf("_body");
          if (babyIndex != -1) {
            prefix = prefix.Substring(0, babyIndex);
          }

          // TODO: Fix animations shared by piantas
        }

        var claimedBcxFiles = unclaimedBcxFiles
                              .Where(bcxFile => bcxFile.Name.StartsWith(prefix))
                              .ToArray();

        foreach (var claimedBcxFile in claimedBcxFiles) {
          unclaimedBcxFiles.Remove(claimedBcxFile);
        }

        bmdAndBcxFiles[bmdFile] = claimedBcxFiles;
      }
      Asserts.True(unclaimedBcxFiles.Count == 0);
      foreach (var (bmdFile, bcxFiles) in bmdAndBcxFiles) {
        this.ExtractModels_(directory, new[] {bmdFile}, bcxFiles, btiFiles);
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
                                             bmdFile.Name.Length -
                                             ".mod".Length);

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

      if (bmdFiles.Count == 1) {
        MessageUtil.LogExtracting(this.logger_, bmdFiles[0]);
      } else {
        MessageUtil.LogExtracting(this.logger_, directory, bmdFiles);
      }

      try {
        new ManualBmd2FbxApi().Process(outputDirectory,
                                       bmdFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       bcxFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       btiFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       !allowMultipleAnimatedModels,
                                       60);
      } catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }
      this.logger_.LogInformation(" ");
    }
  }
}