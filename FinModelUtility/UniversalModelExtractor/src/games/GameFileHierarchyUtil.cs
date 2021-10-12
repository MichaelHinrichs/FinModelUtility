using System.IO;

using fin.io;

using uni.platforms;
using uni.util.io;

namespace uni.games {
  public static class GameFileHierarchyUtil {
    public static string GetRomName(
        IFileHierarchyInstance fileHierarchyInstance) {
      var baseDirectoryPath = fileHierarchyInstance.FullName.Substring(
          0,
          fileHierarchyInstance.FullName.Length -
          fileHierarchyInstance.LocalPath.Length);

      var baseDirectoryName = Path.GetFileName(baseDirectoryPath);

      return baseDirectoryName.Substring(0, baseDirectoryName.IndexOf('.'));
    }

    public static IDirectory GetOutputDirectoryForFile(
        IFileHierarchyFile fileHierarchyFile) {
      var romName = GameFileHierarchyUtil.GetRomName(fileHierarchyFile);

      var localFilePath = fileHierarchyFile.LocalPath;
      var localDirectoryPath =
          localFilePath.Substring(0,
                                  localFilePath.Length -
                                  fileHierarchyFile.Name.Length);

      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.OUT_DIRECTORY
                               .TryToGetSubdir(localOutPath, true);
    }

    public static IDirectory GetOutputDirectoryForDirectory(
        IFileHierarchyDirectory fileHierarchyDirectory) {
      var romName = GameFileHierarchyUtil.GetRomName(fileHierarchyDirectory);

      var localDirectoryPath = fileHierarchyDirectory.LocalPath;
      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.OUT_DIRECTORY
                               .TryToGetSubdir(localOutPath, true);
    }
  }
}