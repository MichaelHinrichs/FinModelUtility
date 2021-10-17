using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using System;
using System.Collections.Generic;
using System.Linq;

using uni.platforms;
using uni.platforms.gcn;
using uni.src.msg;
using uni.util.io;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineExtractor {
    private readonly ILogger logger_ =
        Logging.Create<SuperMarioSunshineExtractor>();

    public void ExtractAll() {
      var superMarioSunshineRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "super_mario_sunshine.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(superMarioSunshineRom);

      this.ExtractMario_(fileHierarchy);
      this.ExtractFludd_(fileHierarchy);
      this.ExtractYoshi_(fileHierarchy);
      this.ExtractScenes_(fileHierarchy);
    }

    private void ExtractMario_(IFileHierarchy fileHierarchy) {
      var marioSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\mario");
      var bcxFiles = marioSubdir.TryToGetSubdir("bck")
                                .Files.Where(
                                    file => file.Name.StartsWith("ma_"))
                                .Select(file => file.Impl)
                                .ToArray();
      var bmdFile = marioSubdir.TryToGetSubdir("bmd")
                               .Files.Single(
                                   file => file.Name == "ma_mdl1.bmd");

      this.ExtractModels_(marioSubdir, new[] {bmdFile}, bcxFiles);
    }

    private void ExtractFludd_(IFileHierarchy fileHierarchy) {
      var fluddSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\mario\watergun2");
      foreach (var subdir in fluddSubdir.Subdirs) {
        this.ExtractPrimaryAndSecondaryModels_(
            subdir,
            file => file.Name.Contains("wg"));
      }
    }

    private void ExtractYoshi_(IFileHierarchy fileHierarchy) {
      var yoshiSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\yoshi");
      var bcxFiles = yoshiSubdir
                     .Files.Where(
                         file => file.Extension == ".bck")
                     // TODO: Look into this, this animation seems to need extra bone(s)?
                     .Where(file => !file.Name.StartsWith("yoshi_tongue"))
                     .Select(file => file.Impl)
                     .ToArray();
      var bmdFile = yoshiSubdir.Files.Single(
          file => file.Name == "yoshi_model.bmd");

      this.ExtractModels_(yoshiSubdir, new[] {bmdFile}, bcxFiles);
    }

    private void ExtractScenes_(IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      foreach (var subdir in sceneSubdir.Subdirs) {
        var mapSubdir = subdir.TryToGetSubdir("map");
        var bmdFiles = mapSubdir.TryToGetSubdir("map")
                                .Files.Where(file => file.Extension == ".bmd")
                                .ToArray();
        this.ExtractModels_(mapSubdir, bmdFiles);

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

          if (objName.StartsWith("montem") && !objName.Contains("common")) {
            Asserts.Nonnull(montemcommon);
            this.ExtractFromSeparateDirectories_(objectSubdir, montemcommon);
          } else if (objName.StartsWith("montew") &&
                     !objName.Contains("common")) {
            this.ExtractFromSeparateDirectories_(objectSubdir, montewcommon);
          } else if (objName.StartsWith("hamukuri")) {
            if (!objName.Contains("anm")) {
              this.ExtractFromSeparateDirectories_(objectSubdir, hamukurianm);
            }
          } else {
            this.ExtractModelsAndAnimationsFromSceneObject_(objectSubdir);
          }
        }
      }
    }

    private void ExtractFromSeparateDirectories_(
        IFileHierarchyDirectory directory,
        IFileHierarchyDirectory common) {
      Asserts.Nonnull(common);

      var bmdFiles = directory.Files.Where(file => file.Extension == ".bmd")
                              .ToArray();
      var commonBcxFiles = common
                           .Files.Where(
                               file => file.Extension == ".bck" ||
                                       file.Extension == ".bca")
                           .Select(file => file.Impl)
                           .ToArray();

      var localBcxFiles = directory.Files.Where(
                                       file => file.Extension == ".bck" ||
                                               file.Extension == ".bca")
                                   .Select(file => file.Impl)
                                   .ToArray();
      if (bmdFiles.Length == 1) {
        this.ExtractModels_(directory,
                            bmdFiles,
                            commonBcxFiles.Concat(localBcxFiles).ToArray());
        return;
      }

      Asserts.True(localBcxFiles.Length == 0);

      this.ExtractPrimaryAndSecondaryModels_(
          directory,
          bmdFile => bmdFile.Name.StartsWith(
              "default"),
          bmdFiles,
          commonBcxFiles);
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
          this.ExtractModels_(directory,
                              new[] {
                                  bmdFile
                              },
                              allBcxFiles);
        }
        return;
      }

      if (directory.Name == "montemcommon" ||
          directory.Name == "montewcommon") {
        foreach (var bmdFile in bmdFiles) {
          this.ExtractModels_(directory,
                              new[] {
                                  bmdFile
                              });
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
      Asserts.True(unclaimedBcxFiles.Count == 0);
      foreach (var (bmdFile, bcxFiles) in bmdAndBcxFiles) {
        this.ExtractModels_(directory, new[] {bmdFile}, bcxFiles);
      }
    }

    private void ExtractPrimaryAndSecondaryModels_(
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
                     .Select(file => file.Impl)
                     .ToArray();

      this.ExtractPrimaryAndSecondaryModels_(directory,
                                             primaryIdentifier,
                                             bmdFiles,
                                             bcxFiles);
    }

    private void ExtractPrimaryAndSecondaryModels_(
        IFileHierarchyDirectory directory,
        Func<IFileHierarchyFile, bool> primaryIdentifier,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFile>? bcxFiles = null
    ) {
      var primaryBmdFile =
          bmdFiles.Single(bmdFile => primaryIdentifier(bmdFile));
      this.ExtractModels_(directory, new[] {primaryBmdFile}, bcxFiles);

      var secondaryBmdFiles = bmdFiles
                              .Where(bmdFile => !primaryIdentifier(bmdFile))
                              .ToArray();
      if (secondaryBmdFiles.Length > 0) {
        this.ExtractModels_(directory, secondaryBmdFiles);
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