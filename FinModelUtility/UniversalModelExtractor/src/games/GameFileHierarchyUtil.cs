﻿using fin.io;

using uni.platforms;

namespace uni.games {
  public static class GameFileHierarchyUtil {
    public static string GetRomName(
        IFileHierarchyInstance fileHierarchyInstance) {
      var baseDirectoryPath = fileHierarchyInstance.FullName.Substring(
          0,
          fileHierarchyInstance.FullName.Length -
          fileHierarchyInstance.LocalPath.Length);

      return Path.GetFileName(baseDirectoryPath);
    }

    public static IDirectory GetOutputDirectoryForFile(
        IFileHierarchyFile fileHierarchyFile)
      => GetOutputDirectoryForDirectory(fileHierarchyFile.Parent!);

    public static IDirectory GetOutputDirectoryForDirectory(
        IFileHierarchyDirectory fileHierarchyDirectory) {
      var romName = GameFileHierarchyUtil.GetRomName(fileHierarchyDirectory);

      var localDirectoryPath = fileHierarchyDirectory.LocalPath;
      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.OUT_DIRECTORY
                               .GetSubdir(localOutPath, true);
    }
  }
}