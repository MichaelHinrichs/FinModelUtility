using fin.io;
using fin.log;
using fin.util.asserts;

using uni.platforms.gcn.tools;
using uni.util.cmd;

namespace uni.platforms.threeDs.tools.ctrtool {
  public static partial class Ctrtool {
    public class CiaExtractor {
      public bool Run(ISystemFile romFile, out IFileHierarchy hierarchy) {
        Asserts.Equal(
            ".cia",
            romFile.FileType,
            $"Cannot dump ROM because it is not a CIA: {romFile}");
        Asserts.True(
            romFile.Exists,
            $"Cannot dump ROM because it does not exist: {romFile}");

        var didChange = false;

        var directory = new FinDirectory(romFile.FullNameWithoutExtension);
        if (!directory.Exists || directory.IsEmpty) {
          didChange = true;

          this.DumpRom_(romFile, directory);
          Asserts.False(directory.IsEmpty,
                        $"Failed to extract contents from the ROM: {romFile.FullPath}");
        }

        hierarchy = new FileHierarchy(directory);
        return didChange;
      }

      private void DumpRom_(ISystemFile romFile,
                            ISystemDirectory dstDirectory) {
        var logger = Logging.Create<CiaExtractor>();
        logger.LogInformation($"Dumping ROM {romFile}...");

        Ctrtool.RunInCtrDirectoryAndCleanUp_(() => {
          ProcessUtil
              .ExecuteBlockingSilently(
                  ThreeDsToolsConstants
                      .EXTRACT_CIA_BAT,
                  $"\"{romFile.FullPath}\"",
                  $"\"{dstDirectory.FullPath}\"");
        });
      }
    }
  }
}