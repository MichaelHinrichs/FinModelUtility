using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.platforms.gcn.tools;
using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.threeDs.tools {
  public class HackingToolkit9ds {
    public bool Run(IFile romFile, out IFileHierarchy hierarchy) {
      Asserts.Equal(
          ".cia",
          romFile
              .Extension,
          $"Cannot dump ROM because it is not a CIA: {romFile}");
      Asserts.True(
          romFile.Exists,
          $"Cannot dump ROM because it does not exist: {romFile}");

      var didChange = false;

      var finalDirectoryPath =
          romFile.FullName.Substring(0,
                                     romFile.FullName.Length - ".cia".Length);
      var finalDirectory = new FinDirectory(finalDirectoryPath);
      if (!finalDirectory.Exists) {
        didChange = true;

        var directoryPath = Path.Join(Path.GetDirectoryName(romFile.FullName),
                                      "ExtractedRomFS");
        var directory = new FinDirectory(directoryPath);

        if (!directory.Exists) {
          this.DumpRom_(romFile);
          Asserts.True(directory.Exists,
                       $"Directory was not created: {directory}");
          this.CleanUpUnneededFiles_();
        }

        Directory.Move(directoryPath, finalDirectoryPath);
        Asserts.True(finalDirectory.Exists,
                     $"Directory was not created: {finalDirectory}");
      }

      hierarchy = new FileHierarchy(finalDirectory);
      return didChange;
    }

    private void DumpRom_(IFile romFile) {
      var logger = Logging.Create<HackingToolkit9ds>();
      logger.LogInformation($"Dumping ROM {romFile}...");

      var processSetup =
          new ProcessUtil.ProcessSetup(ThreeDsToolsConstants
                                           .HACKING_TOOLKIT_9DS_EXE) {
              Method = ProcessUtil.ProcessExecutionMethod.MANUAL,
          };
      var process = ProcessUtil.Execute(processSetup);

      process.StandardInput.WriteLine("CE");

      process.WaitForExit();

      ;
    }

    private void CleanUpUnneededFiles_() {
      var filesToDelete = new string[] {
          "DecryptedExHeader.bin",
          "DecryptedExeFS.bin",
          "DecryptedRomFS.bin",
          "HeaderExeFS.bin",
          "HeaderNCCH0.bin",
          "PlainRGN.bin",
      };
      foreach (var fileToDelete in filesToDelete) {
        ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                             .TryToGetFile(fileToDelete)
                             .Info.Delete();
      }

      var subdirsToDelete = new string[] {
          "ExtractedBanner",
          "ExtractedExeFS",
      };
      foreach (var subdirToDelete in subdirsToDelete) {
        ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                             .TryToGetSubdir(subdirToDelete)
                             .Info.Delete(true);
      }
    }
  }
}