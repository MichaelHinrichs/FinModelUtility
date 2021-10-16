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

      this.ExtractMario_(fileHierarchy);
      this.ExtractScenes_(fileHierarchy);
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

    private void ExtractScenes_(IFileHierarchy fileHierarchy) {
      var sceneSubdir =
          fileHierarchy.Root.TryToGetSubdir(@"data\scene");

      foreach (var subdir in sceneSubdir.Subdirs) {
        var sceneName = subdir.Name;

        var bmd = subdir.TryToGetSubdir(@"map\map")
                        .Files.Single(file => file.Name == "map.bmd");

        new ManualBmd2FbxApi().Process(subdir.Impl,
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