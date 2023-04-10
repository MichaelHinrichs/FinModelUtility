using fin.io;

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

    public static ISystemDirectory GetWorkingDirectoryForFile(
        IFileHierarchyFile fileHierarchyFile,
        string? romName = null)
      => GetWorkingDirectoryForDirectory(fileHierarchyFile.Parent!, romName);

    public static ISystemDirectory GetWorkingDirectoryForDirectory(
        IFileHierarchyDirectory fileHierarchyDirectory,
        string? romName = null) {
      romName ??= GameFileHierarchyUtil.GetRomName(fileHierarchyDirectory);

      var localDirectoryPath = fileHierarchyDirectory.LocalPath;
      var localOutPath = Path.Join(romName, localDirectoryPath);

      return DirectoryConstants.ROMS_DIRECTORY.GetSubdir(localOutPath);
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
                               .GetSubdir(localOutPath, true);
    }
  }
}