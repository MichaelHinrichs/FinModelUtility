using System;
using System.Collections.Generic;

using fin.log;

using uni.util.io;

namespace uni.src.msg {
  public static class MessageUtil {
    public static void LogExtracting<T>(
        ILogger logger,
        IFileHierarchyDirectory directory,
        IReadOnlyList<T> rawModels)
      => logger.LogInformation(
          $"Extracting model" +
          (rawModels.Count != 1 ? "s" : "") +
          $" from {directory.LocalPath}");
  }
}