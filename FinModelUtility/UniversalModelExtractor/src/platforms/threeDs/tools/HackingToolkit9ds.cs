using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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

        var originalFiles = ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                                                 .GetExistingFiles();

        var directoryPath = Path.Join(
            ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY.FullName,
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

      Files.RunInDirectory(
          ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY,
          () => {
            var processStartInfo =
                new ProcessStartInfo(
                    $"\"{ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_EXE.FullName}\"") {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                };

            var process = Process.Start(processStartInfo);
            ChildProcessTracker.AddProcess(process);

            var singleChar = new char[1];

            var output = "";
            while (!output.EndsWith("Write your choice:")) {
              process.StandardOutput.Read(singleChar, 0, 1);
              output += singleChar[0];
            }

            process.StandardInput.WriteLine("CE");

            // For some reason this is needed by HackingToolkit9DS.
            var romPathWithoutExtension =
                romFile.FullName.Substring(0,
                                           romFile.FullName.Length -
                                           ".cia".Length);
            process.StandardInput.WriteLine(romPathWithoutExtension);

            output = "";
            while (!output.EndsWith("Extraction done!")) {
              process.StandardOutput.Read(singleChar, 0, 1);
              output += singleChar[0];
            }

            process.Kill(true);
          });
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
                             .GetExistingFile(fileToDelete)
                             .Info.Delete();
      }

      var subdirsToDelete = new string[] {
          "ExtractedBanner",
          "ExtractedExeFS",
      };
      foreach (var subdirToDelete in subdirsToDelete) {
        ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                             .GetSubdir(subdirToDelete)
                             .Info.Delete(true);
      }
    }
  }
}