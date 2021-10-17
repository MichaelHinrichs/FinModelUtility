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

    private void ExtractScenes_(IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      foreach (var subdir in sceneSubdir.Subdirs) {
        var mapSubdir = subdir.TryToGetSubdir("map");
        var bmdFile = mapSubdir.TryToGetSubdir("map")
                               .Files.Single(file => file.Name == "map.bmd");
        this.ExtractModels_(mapSubdir, new[] {bmdFile});
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
                                       !allowMultipleAnimatedModels,
                                       60);
      } catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }
      this.logger_.LogInformation(" ");
    }
  }
}