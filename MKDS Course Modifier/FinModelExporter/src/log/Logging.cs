using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace fin.log {
  public class Logging {
    private static bool VERBOSE_;

    public static void Initialize(bool verbose) {
      Logging.VERBOSE_ = verbose;
    }

    public static ILogger Create<T>()
      => Logging.VERBOSE_
             ? new Logger<T>(
                 LoggerFactory.Create(builder => {
                   builder.AddConsole();
                   builder.AddDebug();
                 }))
             : new NullLogger<T>();
  }
}