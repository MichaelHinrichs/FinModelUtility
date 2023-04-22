using System;

namespace fin.util.time {
  public static class FrameTime {
    private static DateTime previousFrameStart_;

    public static void MarkStartOfFrame() {
      FrameTime.previousFrameStart_ = FrameTime.StartOfFrame;
      FrameTime.StartOfFrame = DateTime.Now;

      var elapsedSeconds =
          (float) (FrameTime.StartOfFrame - FrameTime.previousFrameStart_)
          .TotalSeconds;
      FrameTime.DeltaTime = elapsedSeconds;
    }

    public static DateTime StartOfFrame { get; private set; }
    public static float DeltaTime { get; private set; }
  }
}