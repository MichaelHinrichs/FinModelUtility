using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.gcn.tools {
  public class RarcDump {
    public bool Run(IFileHierarchyFile rarcFile) {
      Asserts.True(
          rarcFile.Impl.Exists,
          $"Cannot dump RARC because it does not exist: {rarcFile}");
      Asserts.Equal(
          ".rarc",
          rarcFile.Impl.Extension,
          $"Cannot dump file because it is not a RARC: {rarcFile}");

      var directoryPath = rarcFile.FullName + "_dir";
      var didDirectoryExist = Directory.Exists(directoryPath);
      if (!didDirectoryExist) {
        var logger = Logging.Create<RarcDump>();
        using var rarcDumpScope = logger.BeginScope("rarcdump");

        logger.LogInformation($"Dumping RARC {rarcFile.LocalPath}...");

        Files.RunInDirectory(
            rarcFile.Impl.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.RARCDUMP_EXE,
                  $"\"{rarcFile.FullName}\"");
            });
        Asserts.True(Directory.Exists(directoryPath),
                     $"Directory was not created: {directoryPath}");
      }

      return !didDirectoryExist;
    }
  }
}