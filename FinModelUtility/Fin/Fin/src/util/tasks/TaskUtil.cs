using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace fin.util.tasks {
  public static class TaskUtil {
    public static Task<TNumber> Subtract<TNumber>(
        this Task<TNumber> lhsTask,
        Task<TNumber> rhsTask)
        where TNumber : INumber<TNumber>
      => Task.WhenAll(lhsTask, rhsTask)
             .ContinueWith(tasks => tasks.Result[0] - tasks.Result[1]);

    public static async Task<TNumber> Subtract<TNumber>(
        this Task<TNumber> lhsTask,
        TNumber rhs)
        where TNumber : INumber<TNumber> {
      var lhs = await lhsTask;
      return lhs - rhs;
    }


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