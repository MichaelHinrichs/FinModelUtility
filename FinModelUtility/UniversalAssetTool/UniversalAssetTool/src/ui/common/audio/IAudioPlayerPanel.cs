using fin.audio.io;

namespace uni.ui.common.audio {
  public interface IAudioPlayerPanel {
    public IReadOnlyList<IAudioFileBundle>? AudioFileBundles { get; set; }

    public event Action<IAudioFileBundle?> OnChange;
  }
}