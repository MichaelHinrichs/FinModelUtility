using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;

namespace uni.platforms.gcn.tools {
  public class RarcDump {
    public bool Run(IFile rarcFile, IDirectory baseRomDirectory) {
      Asserts.True(
          rarcFile.Exists,
          $"Cannot dump RARC because it does not exist: {rarcFile}");
      Asserts.Equal(
          ".rarc",
          rarcFile.Extension,
          $"Cannot dump file because it is not a RARC: {rarcFile}");

      var directoryPath = rarcFile.FullName + "_dir";
      var didDirectoryExist = Directory.Exists(directoryPath);
      if (!didDirectoryExist) {
        var logger = Logging.Create<RarcDump>();
        using var rarcDumpScope = logger.BeginScope("rarcdump");

        var localRarcFilePath =
            rarcFile.FullName.Substring(baseRomDirectory.FullName.Length);
        logger.LogInformation($"Dumping RARC {localRarcFilePath}...");

        Files.RunInDirectory(
            rarcFile.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.RARCDUMP_EXE,
                  rarcFile.FullName);
            });
        Asserts.True(Directory.Exists(directoryPath),
                     $"Directory was not created: {directoryPath}");
      }

      return !didDirectoryExist;
    }
  }
}