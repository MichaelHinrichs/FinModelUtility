﻿using System.IO;

using fin.io;
using fin.io.bundles;

using sm64.api;

using uni.platforms;

namespace uni.games.super_mario_64 {
  using IAnnotatedSm64Bundle = IAnnotatedFileBundle<Sm64LevelFileBundle>;

  public class SuperMario64AnnotatedFileGatherer :
      IAnnotatedFileBundleGatherer<Sm64LevelFileBundle> {
    public IEnumerable<IAnnotatedSm64Bundle>
        GatherFileBundles() {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "super_mario_64.z64",
              out var superMario64Rom)) {
        yield break;
      }

      ExtractorUtil.GetOrCreateRomDirectories("super_mario_64",
                                              out var prereqsDir,
                                              out var extractedDir);
      var fileHierarchy = new FileHierarchy("super_mario_64", extractedDir);
      var root = fileHierarchy.Root;

      var rootSysDir = root.Impl;
      var levelsDir = rootSysDir.GetOrCreateSubdir("levels");

      var levelIds = Enum.GetValues<LevelId>().ToList();
      levelIds.Sort((lhs, rhs) => lhs.ToString().CompareTo(rhs.ToString()));
      var levelIdsAndPaths = levelIds.Select(levelId => {
        var path = Path.Join(levelsDir.Name, levelId.ToString());
        return (levelId, path);
      });

      foreach (var (_, path) in levelIdsAndPaths) {
        var zObjectFile = new FinFile(Path.Join(rootSysDir.FullPath, path));

        // TODO: Write the actual data here
        FinFileSystem.File.Create(zObjectFile.FullPath);
      }

      root.Refresh(true);
      foreach (var (levelId, path) in levelIdsAndPaths) {
        var levelFile = root.AssertGetExistingFile(path);
        yield return new Sm64LevelSceneFileBundle(
            root,
            superMario64Rom,
            levelId).Annotate(levelFile);
      }

      var marioAnimationsFile = prereqsDir.AssertGetExistingFile("mario_animations.csv");
      // TODO: 
    }
  }
}