using fin.io;

using uni.platforms;

namespace uni.games {
  public static class GameFileHierarchyUtil {
    public static string GetRomName(
        IFileHierarchyInstance fileHierarchyInstance) {
      var baseDirectoryPath = fileHierarchyInstance.FullPath.Substring(
          0,
          fileHierarchyInstance.FullPath.Length -
          fileHierarchyInstance.LocalPath.Length);

      return Path.GetFileName(baseDirectoryPath);
    }

    public static ISystemDirectory GetOrCreateWorkingDirectoryForDirectory(
        IFileHierarchyDirectory fileHierarchyDirectory,
        string? romName = null) {
      romName ??= GameFileHierarchyUtil.GetRomName(fileHierarchyDirectory);

      var localDirectoryPath = fileHierarchyDirectory.LocalPath;
      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.ROMS_DIRECTORY.GetOrCreateSubdir(localOutPath);
    }


    public static ISystemDirectory GetOutputDirectoryForFile(
        IFileHierarchyFile fileHierarchyFile)
      => GetOutputDirectoryForDirectory(fileHierarchyFile.Parent!);

    public static ISystemDirectory GetOutputDirectoryForDirectory(
        IFileHierarchyDirectory fileHierarchyDirectory) {
      var romName = GameFileHierarchyUtil.GetRomName(fileHierarchyDirectory);

      var localDirectoryPath = fileHierarchyDirectory.LocalPath;
      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.OUT_DIRECTORY
                               .GetOrCreateSubdir(localOutPath);
    }
  }
}