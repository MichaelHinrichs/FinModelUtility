using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.gcn.tools {
  public class GcmDump {
    public bool Run(IFile romFile, out IFileHierarchy hierarchy) {
      Asserts.Equal(
          ".gcm",
          romFile
              .Extension,
          $"Cannot dump ROM because it is not a GCM: {romFile}");
      Asserts.True(
          romFile.Exists,
          $"Cannot dump ROM because it does not exist: {romFile}");

      var didChange = false;

      var finalDirectoryPath =
          romFile.FullName.Substring(0,
                                     romFile.FullName.Length - ".gcm".Length);
      var finalDirectory = new FinDirectory(finalDirectoryPath);
      if (!finalDirectory.Exists) {
        didChange = true;

        var directoryPath = romFile.FullName + "_dir";
        var directory = new FinDirectory(directoryPath);

        if (!directory.Exists) {
          this.DumpRom_(romFile);
          Asserts.True(directory.Exists,
                       $"Directory was not created: {directory}");
        }

        Directory.Move(directoryPath, finalDirectoryPath);
        Asserts.True(finalDirectory.Exists,
                     $"Directory was not created: {finalDirectory}");
      }

      hierarchy = new FileHierarchy(finalDirectory);
      return didChange;
    }

    private void DumpRom_(IFile romFile) {
      var logger = Logging.Create<GcmDump>();
      using var gcmDumpScope = logger.BeginScope("gcmdump");
      logger.LogInformation($"Dumping ROM {romFile}...");

      Files.RunInDirectory(
          romFile.GetParent()!,
          () => {
            ProcessUtil.ExecuteBlockingSilently(
                GcnToolsConstants.GCMDUMP_EXE,
                $"\"{romFile.FullName}\"");
          });
    }
  }
}