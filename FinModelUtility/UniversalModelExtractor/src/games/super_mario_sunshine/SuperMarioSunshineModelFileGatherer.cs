using bmd.exporter;
using fin.io;
using fin.io.bundles;
using fin.util.asserts;
using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineModelFileGatherer
      : IFileBundleGatherer<BmdModelFileBundle> {
    public IFileBundleDirectory<BmdModelFileBundle>? GatherFileBundles(
        bool assert) {
      var superMarioSunshineRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "super_mario_sunshine.gcm");
      if (superMarioSunshineRom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard()
                                             .PruneRarcDumpNames("scene");
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, superMarioSunshineRom);

      var rootModelDirectory =
          new FileBundleDirectory<BmdModelFileBundle>("super_mario_sunshine");

      this.ExtractMario_(rootModelDirectory, fileHierarchy);
      this.ExtractFludd_(rootModelDirectory, fileHierarchy);
      this.ExtractYoshi_(rootModelDirectory, fileHierarchy);
      this.ExtractScenes_(rootModelDirectory, fileHierarchy);

      return rootModelDirectory;
    }

    private void ExtractMario_(
        FileBundleDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var marioSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\mario");
      var bcxFiles = marioSubdir.TryToGetSubdir("bck")
                                .Files.Where(
                                    file => file.Name.StartsWith("ma_"))
                                .ToArray();
      var bmdFile = marioSubdir.TryToGetSubdir("bmd")
                               .Files.Single(
                                   file => file.Name == "ma_mdl1.bmd");

      this.ExtractModels_(rootModelDirectory.AddSubdir("mario"),
                          new[] {bmdFile}, bcxFiles);
    }

    private void ExtractFludd_(
        FileBundleDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var fluddSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\mario\watergun2");
      foreach (var subdir in fluddSubdir.Subdirs) {
        this.ExtractPrimaryAndSecondaryModels_(
            rootModelDirectory.AddSubdir("fludd"),
            subdir,
            file => file.Name.Contains("wg"));
      }
    }

    private void ExtractYoshi_(
        FileBundleDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var yoshiSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\yoshi");
      var bcxFiles = yoshiSubdir
                     .Files.Where(
                         file => file.Extension == ".bck")
                     // TODO: Look into this, this animation seems to need extra bone(s)?
                     .Where(file => !file.Name.StartsWith("yoshi_tongue"))
                     .ToArray();
      var bmdFile = yoshiSubdir.Files.Single(
          file => file.Name == "yoshi_model.bmd");

      this.ExtractModels_(rootModelDirectory.AddSubdir("yoshi"),
                          new[] {bmdFile}, bcxFiles);
    }

    private void ExtractScenes_(
        FileBundleDirectory<BmdModelFileBundle> rootModelDirectory,
        IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      foreach (var subdir in sceneSubdir.Subdirs) {
        var sceneNode = rootModelDirectory.AddSubdir(subdir.Name);

        var mapSubdir = subdir.TryToGetSubdir("map");
        var bmdFiles = mapSubdir.TryToGetSubdir("map")
                                .Files.Where(file => file.Extension == ".bmd")
                                .ToArray();
        this.ExtractModels_(sceneNode.AddSubdir("map"), bmdFiles);

        var montemcommon =
            subdir.Subdirs.SingleOrDefault(
                subdir => subdir.Name == "montemcommon");
        var montewcommon =
            subdir.Subdirs.SingleOrDefault(
                subdir => subdir.Name == "montewcommon");
        var hamukurianm =
            subdir.Subdirs.SingleOrDefault(
                subdir => subdir.Name == "hamukurianm");

        foreach (var objectSubdir in subdir.Subdirs) {
          var objName = objectSubdir.Name;
          var objectNode = sceneNode.AddSubdir(objName);

          if (objName.StartsWith("montem") && !objName.Contains("common")) {
            this.ExtractFromSeparateDirectories_(
                objectNode, objectSubdir, Asserts.CastNonnull(montemcommon));
          } else if (objName.StartsWith("montew") &&
                     !objName.Contains("common")) {
            this.ExtractFromSeparateDirectories_(
                objectNode, objectSubdir, Asserts.CastNonnull(montewcommon));
          } else if (objName.StartsWith("hamukuri")) {
            if (!objName.Contains("anm")) {
              this.ExtractFromSeparateDirectories_(
                  objectNode, objectSubdir, Asserts.CastNonnull(hamukurianm));
            }
          } else {
            this.ExtractModelsAndAnimationsFromSceneObject_(
                objectNode, objectSubdir);
          }
        }
      }
    }

    private void ExtractFromSeparateDirectories_(
        IFileBundleDirectory<BmdModelFileBundle> node,
        IFileHierarchyDirectory directory,
        IFileHierarchyDirectory common) {
      Asserts.Nonnull(common);

      var bmdFiles = directory.FilesWithExtension(".bmd")
                              .ToArray();
      var commonBcxFiles = common.FilesWithExtensions(".bca", ".bck")
                                 .ToArray();
      var commonBtiFiles = common.FilesWithExtension(".bti")
                                 .ToArray();

      var localBcxFiles = directory.FilesWithExtensions(".bca", ".bck")
                                   .ToArray();
      if (bmdFiles.Length == 1) {
        this.ExtractModels_(node,
                            bmdFiles,
                            commonBcxFiles.Concat(localBcxFiles).ToArray(),
                            commonBtiFiles);
        return;
      }

      try {
        Asserts.True(localBcxFiles.Length == 0);

        this.ExtractPrimaryAndSecondaryModels_(
            node,
            bmdFile => bmdFile.Name.StartsWith(
                "default"),
            bmdFiles,
            commonBcxFiles);
      } catch {
        ;
      }
    }

    private void ExtractModelsAndAnimationsFromSceneObject_(
        IFileBundleDirectory<BmdModelFileBundle> node,
        IFileHierarchyDirectory directory) {
      var bmdFiles = directory.Files.Where(
                                  file => file.Extension == ".bmd")
                              .OrderByDescending(file => file.Name.Length)
                              .ToArray();

      var allBcxFiles = directory
                        .Files.Where(
                            file => file.Extension == ".bck" ||
                                    file.Extension == ".bca")
                        .ToArray();

      var specialCase = false;
      if (allBcxFiles.Length == 1 &&
          allBcxFiles[0].Name == "fish_swim.bck" &&
          bmdFiles.All(file => file.Name.StartsWith("fish"))) {
        specialCase = true;
      }
      if (allBcxFiles.Length == 1 &&
          allBcxFiles[0].Name == "butterfly_fly.bck" &&
          bmdFiles.All(file => file.Name.StartsWith("butterfly"))) {
        specialCase = true;
      }
      if (allBcxFiles.All(file => file.Name.StartsWith("popo_")) &&
          bmdFiles.All(file => file.Name.StartsWith("popo"))) {
        specialCase = true;
      }

      // If there is only one model or 0 animations, it's easy to tell which
      // animations go with which model.
      if (bmdFiles.Length == 1 || allBcxFiles.Length == 0 || specialCase) {
        foreach (var bmdFile in bmdFiles) {
          this.ExtractModels_(node,
                              new[] {bmdFile},
                              allBcxFiles);
        }
        return;
      }

      if (directory.Name == "montemcommon" ||
          directory.Name == "montewcommon") {
        foreach (var bmdFile in bmdFiles) {
          this.ExtractModels_(node,
                              new[] {bmdFile});
        }
        return;
      }

      var unclaimedBcxFiles = allBcxFiles.ToHashSet();
      var bmdAndBcxFiles =
          new Dictionary<IFileHierarchyFile, IFileHierarchyFile[]>();
      foreach (var bmdFile in bmdFiles) {
        var prefix = bmdFile.Name;
        prefix = prefix.Substring(0, prefix.Length - ".bmd".Length);

        // Blegh. These special cases are gross.
        {
          var modelIndex = prefix.IndexOf("_model");
          if (modelIndex != -1) {
            prefix = prefix.Substring(0, modelIndex);
          }

          var bodyIndex = prefix.IndexOf("_body");
          if (bodyIndex != -1) {
            prefix = prefix.Substring(0, bodyIndex);
          }

          prefix = prefix.Replace("peach_hair_ponytail", "peach_hair_pony");
          prefix = prefix.Replace("eggyoshi_normal", "eggyoshi");
        }

        var claimedBcxFiles = unclaimedBcxFiles
                              .Where(bcxFile => bcxFile.Name.StartsWith(prefix))
                              .ToArray();

        foreach (var claimedBcxFile in claimedBcxFiles) {
          unclaimedBcxFiles.Remove(claimedBcxFile);
        }

        bmdAndBcxFiles[bmdFile] = claimedBcxFiles;
      }
      //Asserts.True(unclaimedBcxFiles.Count == 0);
      foreach (var (bmdFile, bcxFiles) in bmdAndBcxFiles) {
        this.ExtractModels_(node, new[] {bmdFile}, bcxFiles);
      }
    }

    private void ExtractPrimaryAndSecondaryModels_(
        IFileBundleDirectory<BmdModelFileBundle> node,
        IFileHierarchyDirectory directory,
        Func<IFileHierarchyFile, bool> primaryIdentifier
    ) {
      var bmdFiles = directory.Files.Where(
                                  file => file.Extension == ".bmd")
                              .ToArray();
      var bcxFiles = directory
                     .Files.Where(
                         file => file.Extension == ".bck" ||
                                 file.Extension == ".bca")
                     .ToArray();

      this.ExtractPrimaryAndSecondaryModels_(node,
                                             primaryIdentifier,
                                             bmdFiles,
                                             bcxFiles);
    }

    private void ExtractPrimaryAndSecondaryModels_(
        IFileBundleDirectory<BmdModelFileBundle> node,
        Func<IFileHierarchyFile, bool> primaryIdentifier,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFileHierarchyFile>? bcxFiles = null
    ) {
      var primaryBmdFile =
          bmdFiles.Single(bmdFile => primaryIdentifier(bmdFile));
      this.ExtractModels_(node, new[] {primaryBmdFile}, bcxFiles);

      var secondaryBmdFiles = bmdFiles
                              .Where(bmdFile => !primaryIdentifier(bmdFile))
                              .ToArray();
      if (secondaryBmdFiles.Length > 0) {
        this.ExtractModels_(node, secondaryBmdFiles);
      }
    }

    private void ExtractModels_(
        IFileBundleDirectory<BmdModelFileBundle> node,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFileHierarchyFile>? bcxFiles = null,
        IReadOnlyList<IFileHierarchyFile>? btiFiles = null
    ) {
      Asserts.True(bmdFiles.Count > 0);

      foreach (var bmdFile in bmdFiles) {
        node.AddFileBundle(new BmdModelFileBundle {
            BmdFile = bmdFile,
            BcxFiles = bcxFiles,
            BtiFiles = btiFiles,
            FrameRate = 60
        });
      }
    }
  }
}