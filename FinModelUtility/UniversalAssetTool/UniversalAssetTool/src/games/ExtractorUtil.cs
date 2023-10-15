using fin.io;
using fin.io.bundles;

using uni.platforms;

namespace uni.games {
  public static class ExtractorUtil {
    public static ISystemDirectory GetOrCreateRomDirectory(
        IReadOnlyTreeFile romFile)
      => GetOrCreateRomDirectory(romFile.NameWithoutExtension);

    public static ISystemDirectory GetOrCreateRomDirectory(
        string romName)
      => DirectoryConstants.ROMS_DIRECTORY.GetOrCreateSubdir(romName);


    public static ISystemDirectory GetOrCreateExtractedDirectory(
        IReadOnlyTreeFile romFile)
      => GetOrCreateRomDirectory(romFile).GetOrCreateSubdir("extracted");

    public static ISystemDirectory GetOrCreateExtractedDirectory(
        string romName)
      => GetOrCreateRomDirectory(romName).GetOrCreateSubdir("extracted");


    public static bool HasNotBeenExtractedYet(
        IReadOnlyTreeFile romFile,
        out ISystemDirectory extractedDir)
      => HasNotBeenExtractedYet(romFile.NameWithoutExtension,
                                out extractedDir);

    public static bool HasNotBeenExtractedYet(
        string romName,
        out ISystemDirectory extractedDir) {
      extractedDir = GetOrCreateExtractedDirectory(romName);
      return extractedDir.IsEmpty;
    }


    public static ISystemDirectory GetOutputDirectoryForFileBundle(
        IAnnotatedFileBundle annotatedFileBundle)
      => DirectoryConstants
         .OUT_DIRECTORY
         .GetOrCreateSubdir(annotatedFileBundle.GameAndLocalPath);
  }
}