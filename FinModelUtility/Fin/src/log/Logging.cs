using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace fin.log {
  public class Logging {
    private static bool VERBOSE_;

    private static readonly IList<LogLevel> VERBOSE_EXCEPTIONS = new [] {
        LogLevel.Critical,
        LogLevel.Error,
        LogLevel.Warning,
    };

    public static void Initialize(bool verbose) {
      Logging.VERBOSE_ = verbose;
    }

    public static ILogger Create<T>()
      => new Logger<T>(
          LoggerFactory.Create(
              builder =>
                  builder.AddConsole()
                         .AddDebug()
                         .AddFilter(
                             logLevel
                                 => Logging.VERBOSE_ ||
                                    VERBOSE_EXCEPTIONS.Contains(logLevel))
          ));
  }
}