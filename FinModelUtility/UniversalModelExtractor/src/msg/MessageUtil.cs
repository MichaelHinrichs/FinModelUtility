using fin.log;
using fin.io;


namespace uni.msg {
  public static class MessageUtil {
    public static void LogExtracting<T>(
        ILogger logger,
        IFileHierarchyDirectory directory,
        IReadOnlyList<T> rawModels) {
      if (rawModels.Count == 1) {
        logger.LogInformation(
            $"Extracting model {rawModels[0]}");
      } else {
        logger.LogInformation(
            $"Extracting models from {directory.LocalPath}");
      }
    }

    public static void LogExtracting(
        ILogger logger,
        IFileHierarchyFile rawModel)
      => logger.LogInformation(
          $"Extracting model {rawModel.LocalPath}");

    public static void LogAlreadyProcessed<T>(
        ILogger logger,
        IFileHierarchyDirectory directory,
        IReadOnlyList<T> rawModels) {
      if (rawModels.Count == 1) {
        logger.LogInformation(
            $"Already processed model {rawModels[0]}");
      } else {
        logger.LogInformation(
            $"Already processed models from {directory.LocalPath}");
      }
    }
  }
}