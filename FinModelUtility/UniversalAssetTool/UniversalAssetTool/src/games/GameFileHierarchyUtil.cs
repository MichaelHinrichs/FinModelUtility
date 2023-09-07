using fin.io;
using fin.io.bundles;

using uni.platforms;

namespace uni.games {
  public static class GameFileHierarchyUtil {
    public static ISystemDirectory GetOrCreateWorkingDirectoryForDirectory(
        IFileHierarchyIoObject ioObject,
        string? romName = null)
      => DirectoryConstants.ROMS_DIRECTORY.GetOrCreateSubdir(
          $"{romName ?? ioObject.Root.Name}/{ioObject.LocalPath}");


    public static ISystemDirectory GetOutputDirectoryForFileBundle(
        IAnnotatedFileBundle annotatedFileBundle)
      => DirectoryConstants
         .OUT_DIRECTORY
         .GetOrCreateSubdir(annotatedFileBundle.GameAndLocalPath);
  }
}