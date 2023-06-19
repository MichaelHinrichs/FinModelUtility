using fin.io;
using fin.log;
using fin.util.asserts;

using uni.platforms.gcn.tools;
using uni.util.cmd;


namespace uni.platforms.threeDs.tools {
  public class CtrtoolCiaExtractor {
    private static readonly object CTRTOOL_LOCK = new();

    public bool Run(ISystemFile romFile, out IFileHierarchy hierarchy) {
      Asserts.Equal(
          ".cia",
          romFile.Extension,
          $"Cannot dump ROM because it is not a CIA: {romFile}");
      Asserts.True(
          romFile.Exists,
          $"Cannot dump ROM because it does not exist: {romFile}");

      var didChange = false;

      var finalDirectoryPath = romFile.FullNameWithoutExtension;
      var finalDirectory = new FinDirectory(finalDirectoryPath);
      if (!finalDirectory.Exists) {
        didChange = true;

        lock (CtrtoolCiaExtractor.CTRTOOL_LOCK) {
          var beforeFiles = ThreeDsToolsConstants.CTRTOOL_DIRECTORY
                                                 .GetExistingFiles()
                                                 .ToHashSet();
          var beforeSubdirs = ThreeDsToolsConstants
                              .CTRTOOL_DIRECTORY
                              .GetExistingSubdirs()
                              .ToHashSet();

          var directoryPath = Path.Join(
              ThreeDsToolsConstants.CTRTOOL_DIRECTORY.FullName,
              "romfs");
          var directory = new FinDirectory(directoryPath);

          if (!directory.Exists) {
            this.DumpRom_(romFile);
            Asserts.True(directory.Exists,
                         $"Failed to find expected ROM filesystem output directory. There may be something with the following CIA: {romFile.FullName}");
          }

          Directory.Move(directoryPath, finalDirectoryPath);
          Asserts.True(finalDirectory.Exists,
                       $"Directory was not created: {finalDirectory}");

          var afterFiles = ThreeDsToolsConstants.CTRTOOL_DIRECTORY
                                                .GetExistingFiles()
                                                .ToArray();
          var afterSubdirs = ThreeDsToolsConstants.CTRTOOL_DIRECTORY
                                                  .GetExistingSubdirs()
                                                  .ToArray();

          // Cleans up unneeded files & directories
          foreach (var afterFile in afterFiles) {
            if (!beforeFiles.Contains(afterFile)) {
              afterFile.Delete();
            }
          }

          foreach (var afterSubdir in afterSubdirs) {
            if (!beforeSubdirs.Contains(afterSubdir)) {
              afterSubdir.Delete(true);
            }
          }
        }
      }

      hierarchy = new FileHierarchy(finalDirectory);
      return didChange;
    }

    private void DumpRom_(ISystemFile romFile) {
      var logger = Logging.Create<CtrtoolCiaExtractor>();
      logger.LogInformation($"Dumping ROM {romFile}...");

      Files.RunInDirectory(
          ThreeDsToolsConstants.CTRTOOL_DIRECTORY,
          () => {
            ProcessUtil.ExecuteBlockingSilently(
                ThreeDsToolsConstants.EXTRACT_CIA_BAT,
                $"\"{romFile.FullName}\"");
          });
    }
  }
}