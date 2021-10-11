using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using fin.io;
using fin.log;
using fin.util.asserts;

namespace uni.util.cmd {
  public class ProcessUtil {
    public static int ExecuteBlocking(
        IFile exeFile,
        params string[] args) {
      var processSetup = new ProcessSetup(exeFile, args) {
          Method = ProcessExecutionMethod.BLOCK,
      };
      return ProcessUtil.Execute(processSetup).Result;
    }

    public static int ExecuteBlockingSilently(
        IFile exeFile,
        params string[] args) {
      var processSetup = new ProcessSetup(exeFile, args) {
          Method = ProcessExecutionMethod.BLOCK,
          WithLogging = false,
      };
      return ProcessUtil.Execute(processSetup).Result;
    }

    public enum ProcessExecutionMethod {
      BLOCK,
      TIMEOUT,
      ASYNC
    }

    public class ProcessSetup {
      public IFile ExeFile { get; set; }
      public string[] Args { get; set; }

      public ProcessExecutionMethod Method { get; set; } =
        ProcessExecutionMethod.BLOCK;

      public bool WithLogging { get; set; } = true;

      public ProcessSetup(IFile exeFile, params string[] args) {
        this.ExeFile = exeFile;
        this.Args = args;
      }
    }

    public static Task<int> Execute(ProcessSetup processSetup) {
      var exeFile = processSetup.ExeFile;
      Asserts.True(
          exeFile.Exists,
          $"Attempted to execute a program that doesn't exist: {exeFile}");

      var args = processSetup.Args;
      var argString = "";
      for (var i = 0; i < args.Length; ++i) {
        // TODO: Is this safe?
        var arg = args[i];

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

      var logger = Logging.Create(exeFile.FullName);
      if (processSetup.WithLogging) {
        process.OutputDataReceived +=
            (_, args) => logger!.LogInformation(args.Data);
        process.ErrorDataReceived += (_, args) => logger!.LogError(args.Data);
      } else {
        process.OutputDataReceived += (_, _) => {};
        process.ErrorDataReceived += (_, _) => {};
      }

      process.BeginOutputReadLine();
      process.BeginErrorReadLine();

      switch (processSetup.Method) {
        case ProcessExecutionMethod.BLOCK: {
          process.WaitForExit();
          break;
        }

        default:
          throw new NotImplementedException();
      }

      // TODO: https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
      /*
      using var outputWaitHandle = new AutoResetEvent(false);
      using var errorWaitHandle = new AutoResetEvent(false);

      process.OutputDataReceived += (sender, e) => {
        if (e.Data == null) {
          // ReSharper disable once AccessToDisposedClosure
          outputWaitHandle.Set();
        } else {
          logger.LogInformation(e.Data);
        }
      };
      process.ErrorDataReceived += (sender, e) => {
        if (e.Data == null) {
          // ReSharper disable once AccessToDisposedClosure
          errorWaitHandle.Set();
        } else {
          logger.LogError(e.Data);
        }
      };

      process.Start();

      process.BeginOutputReadLine();
      process.BeginErrorReadLine();

      // TODO: Allow passing in timeouts
      if (outputWaitHandle.WaitOne() &&
          errorWaitHandle.WaitOne()) {
        process.WaitForExit();
        // Process completed. Check process.ExitCode here.
      } else {
        // Timed out.
      }*/

      return Task.FromResult(process.ExitCode);
    }
  }
}