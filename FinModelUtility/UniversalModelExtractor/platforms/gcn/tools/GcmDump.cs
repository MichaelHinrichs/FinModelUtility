using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.gcn.tools {
  public class GcmDump {
    public bool Run(IFile romFile, out IFileHierarchy hierarchy) {
      Asserts.True(
          romFile.Exists,
          $"Cannot dump ROM because it does not exist: {romFile}");

      var directoryPath = romFile.FullName + "_dir";
      var directory = new FinDirectory(directoryPath);
      var didDirectoryExist = directory.Exists;
      if (!didDirectoryExist) {
        var logger = Logging.Create<GcmDump>();
        using var gcmDumpScope = logger.BeginScope("gcmdump");
        logger.LogInformation($"Dumping ROM {romFile}...");

        Files.RunInDirectory(
            romFile.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.GCMDUMP_EXE,
                  romFile.FullName);
            });
        Asserts.True(directory.Exists,
                     $"Directory was not created: {directory}");
      }

      hierarchy = new FileHierarchy(directory);
      return !didDirectoryExist;
    }
  }
}