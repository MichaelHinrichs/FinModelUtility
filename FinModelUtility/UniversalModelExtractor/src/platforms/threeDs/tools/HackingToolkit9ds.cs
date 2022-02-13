using System.Diagnostics;
using System.IO;
using System.Linq;
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
          romFile.Extension,
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

        var beforeFiles = ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                                               .GetExistingFiles()
                                               .ToHashSet();
        var beforeSubdirs = ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                                                 .GetExistingSubdirs()
                                                 .ToHashSet();

        var directoryPath = Path.Join(
            ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY.FullName,
            "ExtractedRomFS");
        var directory = new FinDirectory(directoryPath);

        if (!directory.Exists) {
          this.DumpRom_(romFile);
          Asserts.True(directory.Exists,
                       $"Failed to find expected HackingToolkit9ds directory:\n{directory}" +
                       "\n\n" +
                       "This is most likely due to not pre-installing " +
                       "HackingToolkit9ds via the installer:\n" +
                       "cli/tools/HackingToolkit9DSv12/SetupUS.exe");
        }

        Directory.Move(directoryPath, finalDirectoryPath);
        Asserts.True(finalDirectory.Exists,
                     $"Directory was not created: {finalDirectory}");

        var afterFiles = ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                                              .GetExistingFiles()
                                              .ToArray();
        var afterSubdirs = ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY
                                                .GetExistingSubdirs()
                                                .ToArray();


        // Cleans up unneeded files & directories
        foreach (var afterFile in afterFiles) {
          if (!beforeFiles.Contains(afterFile)) {
            afterFile.Info.Delete();
          }
        }

        foreach (var afterSubdir in afterSubdirs) {
          if (!beforeSubdirs.Contains(afterSubdir)) {
            afterSubdir.Info.Delete(true);
          }
        }
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

            var process = Asserts.CastNonnull(Process.Start(processStartInfo));
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
  }
}