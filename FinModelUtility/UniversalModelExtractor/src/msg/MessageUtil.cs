using fin.io;
using fin.log;

namespace uni.msg {
  public static class MessageUtil {
    public static void LogExtracting<T>(
        ILogger logger,
        IFileHierarchyDirectory directory,
        IReadOnlyList<T> rawModels) {
      if (rawModels.Count == 1) {
        LogExtracting(logger, rawModels[0]);
      } else {
        logger.LogInformation(
            $"Extracting models from {directory.LocalPath}");
      }
    }

    public static void LogExtracting<T>(
        ILogger logger,
        T rawModel)
      => logger.LogInformation(
          $"Extracting model {rawModel}");

    public static void LogAlreadyProcessed<T>(
        ILogger logger,
        IFileHierarchyDirectory directory,
        IReadOnlyList<T> rawModels) {
      if (rawModels.Count == 1) {
        LogAlreadyProcessed(logger, rawModels[0]);
      } else {
        logger.LogInformation(
            $"Already processed models from {directory.LocalPath}");
      }
    }

    public static void LogAlreadyProcessed<T>(
        ILogger logger,
        T rawModel) {
      logger.LogInformation(
          $"Already processed model {rawModel}");
    }
  }
}