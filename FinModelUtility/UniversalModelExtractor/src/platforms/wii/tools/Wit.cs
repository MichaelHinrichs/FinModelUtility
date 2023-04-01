using asserts;

using fin.io;
using fin.log;

using uni.util.cmd;


namespace uni.platforms.wii.tools {
  public class Wit {
    public bool Run(IFile romFile, out IFileHierarchy hierarchy) {
      Asserts.Equal(
          ".iso",
          romFile
              .Extension,
          $"Cannot dump ROM because it is not an ISO: {romFile}");
      Asserts.True(
          romFile.Exists,
          $"Cannot dump ROM because it does not exist: {romFile}");

      var didChange = false;

      var finalDirectoryPath = romFile.FullNameWithoutExtension;
      var finalDirectory = new FinDirectory(finalDirectoryPath);
      if (!finalDirectory.Exists) {
        didChange = true;

        this.DumpRom_(romFile);
        Asserts.True(finalDirectory.Exists,
                     $"Directory was not created: {finalDirectory}");
      }

      hierarchy = new FileHierarchy(finalDirectory);
      return didChange;
    }

    private void DumpRom_(IFile romFile) {
      var logger = Logging.Create<Wit>();
      logger.LogInformation($"Dumping ROM {romFile}...");

      Files.RunInDirectory(
          romFile.GetParent()!,
          () => {
            ProcessUtil.ExecuteBlockingSilently(
                WiiToolsConstants.WIT_EXE,
                $"extract \"{romFile.FullName}\" \"./{romFile.NameWithoutExtension}\"");
          });
    }
  }
}