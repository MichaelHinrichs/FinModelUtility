using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace fin.util.tasks {
  public static class TaskUtil {
    public static Task RunExpensiveButAccurateTickHandler(
        double frequency,
        Action handler,
        CancellationToken? cancellationToken = null)
      => Task.Run(() => {
        var stopwatch = new Stopwatch();
        var targetPeriod = 1 / frequency;
        var targetTicks = Stopwatch.Frequency * targetPeriod;

        while (!cancellationToken?.IsCancellationRequested ?? true) {
          stopwatch.Restart();

          handler();

          // Very expensive, but FAR more accurate than Thread.sleep
          var i = 0;
          while (stopwatch.ElapsedTicks < targetTicks) {
            ++i;
          }
        }
      });
  }
}