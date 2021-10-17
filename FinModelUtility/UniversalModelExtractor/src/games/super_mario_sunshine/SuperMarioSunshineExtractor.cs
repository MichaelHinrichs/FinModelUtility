using System.Collections.Generic;
using System.Linq;

using bmd.api;

using fin.io;

using uni.platforms;
using uni.platforms.gcn;
using uni.platforms.gcn.tools;
using uni.util.io;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineExtractor {
    public void ExtractAll() {
      var superMarioSunshineRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "super_mario_sunshine.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(superMarioSunshineRom);

      this.Debug_(fileHierarchy);

      //this.ExtractMario_(fileHierarchy);
      //this.ExtractScenes_(fileHierarchy);
    }

    private void ExtractMario_(IFileHierarchy fileHierarchy) {
      var marioSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\mario");
      var marioBmd = marioSubdir.TryToGetSubdir("bmd")
                                .Files.Single(
                                    file => file.Name == "ma_mdl1.bmd");

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(marioSubdir);

      new ManualBmd2FbxApi().Process(outputDirectory,
                                     new[] {
                                         marioBmd.FullName
                                     },
                                     new List<string>(),
                                     new List<string>(),
                                     true,
                                     60);
    }

    private void Debug_(IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      //var specificSubdir = sceneSubdir.TryToGetSubdir("bianco1_scene");
      var specificSubdir = sceneSubdir.TryToGetSubdir("dolpic0_scene");

      var bmd = specificSubdir.TryToGetSubdir(@"map\map")
                              .Files.Single(file => file.Name == "map.bmd");

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(specificSubdir);

      new ManualBmd2FbxApi().Process(outputDirectory,
                                     new[] {
                                         bmd.FullName
                                     },
                                     new List<string>(),
                                     new List<string>(),
                                     true);
    }

    private void ExtractScenes_(IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      foreach (var subdir in sceneSubdir.Subdirs) {
        var bmd = subdir.TryToGetSubdir(@"map\map")
                        .Files.Single(file => file.Name == "map.bmd");

        var outputDirectory =
            GameFileHierarchyUtil.GetOutputDirectoryForDirectory(subdir);

        new ManualBmd2FbxApi().Process(outputDirectory,
                                       new[] {
                                           bmd.FullName
                                       },
                                       new List<string>(),
                                       new List<string>(),
                                       true);
      }
    }
  }
}