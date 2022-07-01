using bmd.exporter;

using fin.io;
using fin.model;
using fin.util.asserts;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.mario_kart_double_dash {
  public class
      MarioKartDoubleDashFileGatherer : IModelFileGatherer<BmdModelFileBundle> {
    public IModelDirectory<BmdModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var marioKartDoubleDashRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "mario_kart_double_dash.gcm");
      if (marioKartDoubleDashRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard()
                                             .UseRarcDumpForExtensions(".arc");
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, marioKartDoubleDashRom);

      var rootModelDirectory =
          new ModelDirectory<BmdModelFileBundle>("mario_kart_double_dash");

      this.ExtractDrivers_(rootModelDirectory, fileHierarchy);
      this.ExtractKarts_(rootModelDirectory, fileHierarchy);
      this.ExtractCourses_(rootModelDirectory, fileHierarchy);
      // TODO: Extract "enemies"
      // TODO: Extract "objects"

      return rootModelDirectory;
    }

    private void ExtractKarts_(
        ModelDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var kartSubdir = fileHierarchy.Root.TryToGetSubdir(@"MRAM\kart");
      var kartNode = rootModelDirectory.AddSubdir("kart");

      foreach (var subdir in kartSubdir.Subdirs) {
        var bmdFiles = subdir.FilesWithExtension(".bmd")
                             .ToArray();
        this.ExtractModels_(kartNode.AddSubdir(subdir.Name), subdir, bmdFiles);
      }
    }

    private void ExtractDrivers_(
        ModelDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var mramSubdir = fileHierarchy.Root.TryToGetSubdir(@"MRAM\driver");
      var driverNode = rootModelDirectory.AddSubdir("driver");

      {
        var plumberNames = new[] {"mario", "luigi",};
        var plumberSubdirs =
            mramSubdir.Subdirs.Where(
                subdir => plumberNames.Contains(subdir.Name));
        var plumberCommon = mramSubdir.TryToGetSubdir("cmn_hige");
        foreach (var plumberSubdir in plumberSubdirs) {
          this.ExtractFromSeparateDriverDirectories_(
              driverNode.AddSubdir(plumberSubdir.Name),
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
              driverNode.AddSubdir(babySubdir.Name),
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
              driverNode.AddSubdir(princessSubdir.Name),
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
              driverNode.AddSubdir(lizardSubdir.Name),
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
              driverNode.AddSubdir(koopaSubdir.Name),
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
          this.ExtractFromDriverDirectory_(
              driverNode.AddSubdir(standaloneSubdir.Name),
              standaloneSubdir);
        }
      }
    }

    private void ExtractFromDriverDirectory_(
        IModelDirectory<BmdModelFileBundle> node,
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
      this.ExtractModels_(node,
                          directory,
                          driverBmdFiles,
                          driverBcxFiles);

      var otherBmdFiles = bmdFiles.Where(file => !driverBmdFiles.Contains(file))
                                  .ToArray();
      if (otherBmdFiles.Length > 0) {
        var otherBcxFiles =
            bcxFiles.Where(file => !driverBcxFiles.Contains(file))
                    .ToArray();
        this.ExtractModels_(node,
                            directory,
                            otherBmdFiles,
                            otherBcxFiles);
      }
    }

    private void ExtractFromSeparateDriverDirectories_(
        IModelDirectory<BmdModelFileBundle> node,
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

      this.ExtractModels_(node,
                          directory,
                          bmdFiles,
                          commonBcxFiles.Concat(localBcxFiles).ToArray());
    }

    private void ExtractCourses_(
        ModelDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var courseSubdir = fileHierarchy.Root.TryToGetSubdir("Course");
      var coursesNode = rootModelDirectory.AddSubdir("Course");

      foreach (var subdir in courseSubdir.Subdirs) {
        var bmdFiles = subdir.FilesWithExtension(".bmd")
                             .ToArray();
        if (bmdFiles.Length == 0) {
          continue;
        }

        var btiFiles = subdir.FilesWithExtension(".bti")
                             .Select(file => file.Impl)
                             .ToArray();

        var courseNode = coursesNode.AddSubdir(subdir.Name);
        this.ExtractModels_(courseNode,
                            subdir,
                            bmdFiles,
                            null,
                            btiFiles);

        var objectsSubdir = subdir.TryToGetSubdir("objects");
        this.ExtractModelsAndAnimationsFromSceneObject_(
            courseNode.AddSubdir("objects"),
            objectsSubdir);
      }
    }

    private void ExtractModelsAndAnimationsFromSceneObject_(
        IModelDirectory<BmdModelFileBundle> node,
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
          this.ExtractModels_(node,
                              directory,
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
        this.ExtractModels_(node, directory, new[] {bmdFile}, bcxFiles, btiFiles);
      }
    }

    private void ExtractModels_(
        IModelDirectory<BmdModelFileBundle> node,
        IFileHierarchyDirectory directory,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFile>? bcxFiles = null,
        IReadOnlyList<IFile>? btiFiles = null
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
        node.AddFileBundle(new BmdModelFileBundle {
            BmdFile = bmdFile.Impl,
            BcxFiles = bcxFiles,
            BtiFiles = btiFiles,
            FrameRate = 60
        });
      }
    }
  }
}