using System.Diagnostics;

using fin.io;
using fin.log;
using fin.util.asserts;

namespace uni.util.cmd {
  public class ProcessUtil {
    public static int ExecuteBlocking(IFile exeFile, params string[] args) {
      Asserts.True(
          exeFile.Exists,
          $"Attempted to execute a program that doesn't exist: {exeFile}");

      var argString = "";
      for (var i = 0; i < args.Length; ++i) {
        // TODO: Is this safe?
        var arg = args[i];
        Asserts.False(
            arg.Contains("\""),
            $"While attempting to execute ${exeFile}, received an argument " +
            $"containing a quote. This is not safe: {arg}");

        if (i > 0) {
          argString += " ";
        }
        argString += arg;
      }

      var processStartInfo =
          new ProcessStartInfo(exeFile.FullName, argString) {
              CreateNoWindow = true,
              RedirectStandardOutput = true,
              RedirectStandardError = true,
              UseShellExecute = false,
          };

      var process = Process.Start(processStartInfo);
      ChildProcessTracker.AddProcess(process);

      var logger = Logging.Instance;
      process.OutputDataReceived +=
          (_, args) => logger!.LogInformation(args.Data);
      process.ErrorDataReceived += (_, args) => logger!.LogError(args.Data);

      process.BeginOutputReadLine();
      process.BeginErrorReadLine();

      process.WaitForExit();

      return process.ExitCode;
    }
  }
}